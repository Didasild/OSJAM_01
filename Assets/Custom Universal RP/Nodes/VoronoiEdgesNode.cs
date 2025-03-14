using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{ 
	[Title("Procedural", "Noise", "Voronoi Edges")]
    class VoronoiEdgesNode : AbstractMaterialNode, IGeneratesBodyCode, IGeneratesFunction, IMayRequireMeshUV
    {
        // 0 original version
        // 1 add deterministic noise option
        public override int latestVersion => 1;
        public override IEnumerable<int> allowedNodeVersions => new[] { 1 };

        public const int UVSlotId = 0;
        public const int AngleOffsetSlotId = 1;
        public const int CellDensitySlotId = 2;
        public const int OutSlotId = 3;
        public const int CellsSlotId = 4;

        const string kUVSlotName = "UV";
        const string kAngleOffsetSlotName = "AngleOffset";
        const string kCellDensitySlotName = "CellDensity";
        const string kOutSlotName = "Out";
        const string kCellsSlotName = "Cells";

        public VoronoiEdgesNode()
        {
            name = "Voronoi Edges";
            synonyms = new[] { "worley noise" };
            UpdateNodeAfterDeserialization();
        }

        public enum HashType
        {
            Deterministic,
            LegacySine,
        };
        static readonly string[] kHashFunctionPrefix =
        {
            "Hash_Tchou_2_2_",
            "Hash_LegacySine_2_2_",
        };

        public override bool hasPreview => true;

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new UVMaterialSlot(UVSlotId, kUVSlotName, kUVSlotName, UVChannel.UV0));
            AddSlot(new Vector1MaterialSlot(AngleOffsetSlotId, kAngleOffsetSlotName, kAngleOffsetSlotName, SlotType.Input, 2.0f));
            AddSlot(new Vector1MaterialSlot(CellDensitySlotId, kCellDensitySlotName, kCellDensitySlotName, SlotType.Input, 5.0f));
            AddSlot(new Vector1MaterialSlot(OutSlotId, kOutSlotName, kOutSlotName, SlotType.Output, 0.0f));
            AddSlot(new Vector1MaterialSlot(CellsSlotId, kCellsSlotName, kCellsSlotName, SlotType.Output, 0.0f));

            RemoveSlotsNameNotMatching(new[] { UVSlotId, AngleOffsetSlotId, CellDensitySlotId, OutSlotId, CellsSlotId });
        }

        [SerializeField]
        private HashType m_HashType = HashType.Deterministic;

        [EnumControl("Hash Type")]
        public HashType hashType
        {
            get
            {
                if (((int)m_HashType < 0) || ((int)m_HashType >= kHashFunctionPrefix.Length))
                    return (HashType)0;
                return m_HashType;
            }
            set
            {
                if (m_HashType == value)
                    return;

                m_HashType = value;
                Dirty(ModificationScope.Graph);
            }
        }

        void IGeneratesFunction.GenerateNodeFunction(FunctionRegistry registry, GenerationMode generationMode)
        {
            registry.RequiresIncludePath("Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl");

            HashType hashType = this.hashType;
            string hashTypeString = hashType.ToString();
            string HashFunction = kHashFunctionPrefix[(int)hashType];

            registry.ProvideFunction($"Custom_VoronoiEdges_RandomVector_{hashTypeString}_$precision", s =>
            {
                s.AppendLine($"$precision2 Custom_VoronoiEdges_RandomVector_{hashTypeString}_$precision ($precision2 UV, $precision offset)");
                using (s.BlockScope())
                {
                    s.AppendLine($"{HashFunction}$precision(UV, UV);");
                    s.AppendLine("return $precision2(sin(UV.y * offset), cos(UV.x * offset)) * 0.5 + 0.5;");
                }
            });

            registry.ProvideFunction($"Custom_VoronoiEdges_{hashTypeString}_$precision", s =>
            {
	            s.AppendLine($"void Custom_VoronoiEdges_{hashTypeString}_$precision($precision2 UV, $precision AngleOffset, $precision CellDensity, out $precision Out, out $precision Cells)");
	            using (s.BlockScope())
	            {
		            s.AppendLine("$precision2 g = floor(UV * CellDensity);");
		            s.AppendLine("$precision2 f = frac(UV * CellDensity);");
		            s.AppendLine("$precision3 res1 = $precision3(8.0, 0.0, 0.0);");
		            s.AppendLine("$precision3 res2 = $precision3(8.0, 0.0, 0.0);");

		            s.AppendLine("for (int y = -1; y <= 1; y++)");
		            using (s.BlockScope())
		            {
			            s.AppendLine("for (int x = -1; x <= 1; x++)");
			            using (s.BlockScope())
			            {
				            s.AppendLine("$precision2 lattice = $precision2(x, y);");
				            s.AppendLine($"$precision2 offset = Custom_VoronoiEdges_RandomVector_{hashTypeString}_$precision(lattice + g, AngleOffset);");
				            s.AppendLine("$precision d = distance(lattice + offset, f);");

				            s.AppendLine("if (d < res1.x)");
				            using (s.BlockScope())
				            {
					            s.AppendLine("res2 = res1;");
					            s.AppendLine("res1 = $precision3(d, offset.x, offset.y);");
				            }
				            s.AppendLine("else if (d < res2.x)");
				            using (s.BlockScope())
				            {
					            s.AppendLine("res2 = $precision3(d, offset.x, offset.y);");
				            }
			            }
		            }
		            
		            s.AppendLine("Out = saturate((res2.x - res1.x - 0.05) / 0.05);");
		            s.AppendLine("Cells = res1.y;");
	            }
            });
        }

        public void GenerateNodeCode(ShaderStringBuilder sb, GenerationMode generationMode)
        {
            HashType hashType = this.hashType;
            string hashTypeString = hashType.ToString();
            string uv = GetSlotValue(UVSlotId, generationMode);
            string angleOffset = GetSlotValue(AngleOffsetSlotId, generationMode);
            string cellDensity = GetSlotValue(CellDensitySlotId, generationMode);
            string output = GetVariableNameForSlot(OutSlotId);
            string cells = GetVariableNameForSlot(CellsSlotId);

            sb.AppendLine($"{FindSlot<MaterialSlot>(OutSlotId).concreteValueType.ToShaderString()} {output};");
            sb.AppendLine($"{FindSlot<MaterialSlot>(CellsSlotId).concreteValueType.ToShaderString()} {cells};");
            sb.AppendLine($"Custom_VoronoiEdges_{hashTypeString}_$precision({uv}, {angleOffset}, {cellDensity}, {output}, {cells});");
        }

        public bool RequiresMeshUV(UVChannel channel, ShaderStageCapability stageCapability)
        {
            using (PooledList<MaterialSlot> tempSlots = PooledList<MaterialSlot>.Get())
            {
                GetInputSlots(tempSlots);
                bool result = false;
                foreach (MaterialSlot slot in tempSlots)
                {
                    if (slot.RequiresMeshUV(channel))
                    {
                        result = true;
                        break;
                    }
                }

                tempSlots.Clear();
                return result;
            }
        }

        public override void OnAfterMultiDeserialize(string json)
        {
            if (sgVersion < 1)
            {
                // old nodes should select "LegacySine" to replicate old behavior
                hashType = HashType.LegacySine;
                ChangeVersion(1);
            }
        }
    }
}
