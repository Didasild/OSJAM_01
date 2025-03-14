using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph
{
	[Title("Procedural", "Noise", "Custom Voronoi")]
	internal class CustomVoronoiNode : AbstractMaterialNode,
		IGeneratesBodyCode,
		IGeneratesFunction,
		IMayRequireMeshUV
	{
		public const int UVSlotId = 0;
		public const int AngleOffsetSlotId = 1;
		public const int CellDensitySlotId = 2;
		public const int OutSlotId = 3;
		public const int EdgesSlotId = 4;
		public const int CellsSlotId = 5;

		const string kUVSlotName   = "UV";
		const string kAngleOffsetSlotName= "AngleOffset";
		const string kCellDensitySlotName= "CellDensity";
		const string kOutSlotName = "Out";
		const string kEdgesSlotName = "Edges";
		const string kCellsSlotName = "Cells";

		public CustomVoronoiNode()
		{
			name = "Custom Voronoi";
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
			"Hash_LegacySine_2_2_"
		};

		public override bool hasPreview => true;

		public sealed override void UpdateNodeAfterDeserialization()
		{
			AddSlot(new UVMaterialSlot(UVSlotId, kUVSlotName, kUVSlotName, UVChannel.UV0));
			AddSlot(new Vector1MaterialSlot(AngleOffsetSlotId, kAngleOffsetSlotName, kAngleOffsetSlotName, SlotType.Input, 2.0f));
			AddSlot(new Vector1MaterialSlot(CellDensitySlotId, kCellDensitySlotName, kCellDensitySlotName, SlotType.Input, 5.0f));

			AddSlot(new Vector1MaterialSlot(OutSlotId, kOutSlotName, kOutSlotName, SlotType.Output, 0.0f));
			AddSlot(new Vector1MaterialSlot(EdgesSlotId, kEdgesSlotName, kEdgesSlotName, SlotType.Output, 0.0f));
			AddSlot(new Vector1MaterialSlot(CellsSlotId, kCellsSlotName, kCellsSlotName, SlotType.Output, 0.0f));

			RemoveSlotsNameNotMatching(new[] { UVSlotId, AngleOffsetSlotId, CellDensitySlotId, OutSlotId, EdgesSlotId, CellsSlotId });
		}

		private HashType _hashTypeEnum = HashType.Deterministic;
		
		[EnumControl("Hash Type")]
		public HashType HashTypeEnum
		{
			get
			{
				if ((int)_hashTypeEnum < 0 || (int)_hashTypeEnum >= kHashFunctionPrefix.Length) return 0;
				return _hashTypeEnum;
			}
			set
			{
				if (_hashTypeEnum == value) return;
				_hashTypeEnum = value;
				Dirty(ModificationScope.Graph);
			}
		}
		
		void IGeneratesFunction.GenerateNodeFunction(FunctionRegistry registry, GenerationMode generationMode)
		{
			registry.RequiresIncludePath("Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl");

			string hashStr = kHashFunctionPrefix[(int)HashTypeEnum];
			string hashTypeString = HashTypeEnum.ToString();

			registry.ProvideFunction($"CustomVoronoi_Random_{hashTypeString}_$precision", sb =>
			{
				sb.AppendLine($"$precision2 CustomVoronoi_Random_{hashTypeString}_$precision($precision2 UV, $precision angle)");
				using (sb.BlockScope())
				{ 
					sb.AppendLine($"{hashStr}$precision(UV, UV);");
					sb.AppendLine("return $precision2(sin(UV.y * angle), cos(UV.x * angle)) * 0.5 + 0.5;");
				}
			});
			
			registry.ProvideFunction($"CustomVoronoi_{hashTypeString}_$precision", sb =>
			{
				sb.AppendLine($"void CustomVoronoi_{hashTypeString}_$precision(");
				sb.AppendLine(" $precision2 UV, $precision angle, $precision cellDensity," +
				              "out $precision outDist, out $precision cellsVal, out $precision edgeVal)");
				using (sb.BlockScope())
				{ 
					sb.AppendLine("$precision2 g = floor(UV * cellDensity);");
					sb.AppendLine("$precision2 f = frac(UV * cellDensity);");
					
					sb.AppendLine("$precision nearestDist = 8.0;");
					sb.AppendLine("$precision2 nearestSite = $precision2(0.0, 0.0);");
					sb.AppendLine("$precision nearestOffsetX = 0.0;");
					
					sb.AppendLine("for (int yy = -1; yy <= 1; yy++)");
					using (sb.BlockScope())
					{
						sb.AppendLine("for (int xx = -1; xx <= 1; xx++)");
						using (sb.BlockScope())
						{
							sb.AppendLine("$precision2 lattice = $precision2(xx, yy);");
							sb.AppendLine($"$precision2 offset = CustomVoronoi_Random_{hashTypeString}_$precision(lattice + g, angle);");
							sb.AppendLine("$precision d = distance(lattice + offset, f);");

							sb.AppendLine("if (d < nearestDist)");
							using (sb.BlockScope())
							{
								sb.AppendLine("nearestDist = d;");
								sb.AppendLine("nearestSite = lattice + offset;");
								sb.AppendLine("nearestOffsetX= offset.x;");
							}
						}
					}

					sb.AppendLine("outDist = nearestDist;");
					sb.AppendLine("cellsVal = nearestOffsetX;");
					
					sb.AppendLine("$precision edgeDist = 8.0;");

					sb.AppendLine("for (int yy = -1; yy <= 1; yy++)");
					using (sb.BlockScope())
					{
						sb.AppendLine("for (int xx = -1; xx <= 1; xx++)");
						using (sb.BlockScope())
						{
							sb.AppendLine("$precision2 lattice = $precision2(xx, yy);");
							sb.AppendLine($"$precision2 offset = CustomVoronoi_Random_{hashTypeString}_$precision(lattice + g, angle);");
							sb.AppendLine("$precision2 sitePos = lattice + offset;");
							
							sb.AppendLine("$precision2 midpoint = 0.5 * (nearestSite + sitePos);");
							sb.AppendLine("$precision2 normal = normalize(sitePos - nearestSite);");
							sb.AppendLine("$precision distToEdge = dot(midpoint - f, normal);");
							sb.AppendLine("edgeDist = min(edgeDist, distToEdge);");
						}
					}

					sb.AppendLine("edgeVal = edgeDist;");
				}
			});
		}
		
		public void GenerateNodeCode(ShaderStringBuilder sb, GenerationMode generationMode)
		{
			string hashTypeString = HashTypeEnum.ToString();
			string uv = GetSlotValue(UVSlotId, generationMode);
			string angleOffset = GetSlotValue(AngleOffsetSlotId, generationMode);
			string cellDensity = GetSlotValue(CellDensitySlotId, generationMode);

			string outVar = GetVariableNameForSlot(OutSlotId);
			string cellsVar = GetVariableNameForSlot(CellsSlotId);
			string edgeVar = GetVariableNameForSlot(EdgesSlotId);
   
			sb.AppendLine($"{FindSlot<MaterialSlot>(OutSlotId).concreteValueType.ToShaderString()} {outVar};");
			sb.AppendLine($"{FindSlot<MaterialSlot>(CellsSlotId).concreteValueType.ToShaderString()} {cellsVar};");
			sb.AppendLine($"{FindSlot<MaterialSlot>(EdgesSlotId).concreteValueType.ToShaderString()} {edgeVar};");
   
			sb.AppendLine($"CustomVoronoi_{hashTypeString}_$precision({uv}, {angleOffset}, {cellDensity}, {outVar}, {cellsVar}, {edgeVar});");
		}

		public bool RequiresMeshUV(UVChannel channel, ShaderStageCapability stageCapability)
		{
			using (PooledList<MaterialSlot> tempSlots = PooledList<MaterialSlot>.Get())
			{
				GetInputSlots(tempSlots);
				bool result = false;
				foreach (MaterialSlot slot in tempSlots)
				{
					if (slot.RequiresMeshUV(channel) == false) continue;
					
					result = true;
					break;
				}
				tempSlots.Clear();
				return result;
			}
		}
	}
}