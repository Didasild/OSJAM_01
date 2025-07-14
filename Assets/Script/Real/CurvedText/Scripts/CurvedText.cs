using System;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
[RequireComponent(typeof(TextMeshProUGUI))]
[ExecuteAlways]
public class CurvedText : MonoBehaviour
{
    private TMP_Text textComponent;  // Text component to curve
    private TextController _textController;

    [SerializeField, Range(-100f, 100f)]
    [Tooltip("Radius of the curve (higher values = less curve)")]
    private float curve = 30;

    [SerializeField, Range(5f, 100f)]
    [Tooltip("Fixed arc length per character (spacing between characters along the curve)")]
    private float spacing = 30f;

    [SerializeField]
    private float flatnessThreshold = 4000f;

    [SerializeField, Range(-360f, 360f)]
    [Tooltip("Offset to rotate the arc around the center (in degrees)")]
    private float angularOffset = 0f;
    [SerializeField] private float angularSpeed = 1f;

    private TMP_TextInfo textInfo;
    private float radius;
    private string lastText;
    private float lastCurve, lastSpacing, lastAngularOffset, lastFlatnessThreshold;
    private bool isUpdating = false;

    public void Init(TextController textController)
    {
        _textController = textController;
        textComponent = GetComponent<TMP_Text>();
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!textComponent) textComponent = GetComponent<TMP_Text>();

        EditorApplication.update -= ForceUpdate;
        EditorApplication.update += ForceUpdate;
    }

    private void ForceUpdate()
    {
        if (textComponent == null) return;

        textComponent.ForceMeshUpdate();
        ModifyTextMesh(textComponent.textInfo);
    }
#endif

    private void OnEnable()
    {
        textComponent = GetComponent<TMP_Text>();
        
        if (textComponent != null)
        {
            textComponent.OnPreRenderText -= ModifyTextMesh; // Remove in case it's already added
            textComponent.OnPreRenderText += ModifyTextMesh;
        }
    }
    
    private void OnDisable()
    {
        if (textComponent != null)
        {
            textComponent.OnPreRenderText -= ModifyTextMesh;
        }
    }

    private void Update()
    {
        if (textComponent == null) return;
        if (!Application.isPlaying) return;
        
        angularOffset += angularSpeed * Time.deltaTime;
        if (angularOffset > 155)
        {
            angularOffset -= 515f;
        }
        else if (angularOffset < -360f)
        {
            angularOffset += 515f;
        }

        if (textComponent != null)
        {
            textComponent.ForceMeshUpdate();
            ModifyTextMesh(textComponent.textInfo);
        }
    }

    private void ModifyTextMesh(TMP_TextInfo textInfo)
    {
        if (isUpdating) return;

        isUpdating = true;
        this.textInfo = textInfo;
        UpdateTextCurve();
        isUpdating = false;
    }


    private void UpdateTextCurve()
    {
        if (textComponent == null) return;

        if (curve != 0){
            radius = flatnessThreshold/curve;
        }else{
            radius = flatnessThreshold/0.001f;
        }
        
        textComponent.ForceMeshUpdate();
        textInfo = textComponent.textInfo;
        
        if (textInfo == null) return;
            
        int characterCount = textInfo.characterCount;
        
        if (characterCount == 0) return;
        
        float totalArcLength = spacing * (characterCount - 1);
        float anglePerCharacter = totalArcLength / Mathf.Abs(radius) * Mathf.Rad2Deg;

        for (int i = 0; i < characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            // Get the index and character vertices
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // Calculate character midpoint and offsets
            Vector3 charMidBaselinePos = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, textInfo.characterInfo[i].baseLine);
            vertices[vertexIndex + 0] -= charMidBaselinePos;
            vertices[vertexIndex + 1] -= charMidBaselinePos;
            vertices[vertexIndex + 2] -= charMidBaselinePos;
            vertices[vertexIndex + 3] -= charMidBaselinePos;

            // Calculate angle offset for each character
            float charAngle = (angularOffset + (-totalArcLength / 2f) + i * spacing) / Mathf.Abs(radius) * Mathf.Rad2Deg;

            // Check if curvature is too small
            if (Mathf.Abs(radius) > flatnessThreshold)
            {
                charAngle = -charAngle;
            }

            // Calculate the character's new position along the circular path
            float angleRadians = charAngle * Mathf.Deg2Rad;

            if(curve < 1) angleRadians = -angleRadians;

            Vector3 offset = new Vector3(Mathf.Sin(angleRadians) * radius, Mathf.Cos(angleRadians) * radius, 0) - new Vector3(0f, radius, 0f);

            // Handle negative radius to flip the curvature
            Quaternion rotation = Quaternion.Euler(0, 0, radius > 0 ? -charAngle : charAngle);

            // Apply the transformation matrix to the vertices
            Matrix4x4 matrix = Matrix4x4.TRS(offset, rotation, Vector3.one);

            vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
            vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
            vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
            vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);
        }

        // Update the mesh with the new vertex positions
        textComponent.UpdateVertexData();
    }
}
