using UnityEditor;
using UnityEngine;

public class EditorGUILayoutExhaustiveWindow : EditorWindow
{
    private Vector2 scrollPosition; // Position pour le ScrollView
    private string textField = "Default Text";
    private int intField = 42;
    private float floatField = 3.14f;
    private bool toggleField = true;
    private Color colorField = Color.red;
    private Vector2 vector2Field = Vector2.one;
    private Vector3 vector3Field = Vector3.one;
    private Vector4 vector4Field = Vector4.one;
    private Rect rectField = new Rect(0, 0, 100, 50);
    private Bounds boundsField = new Bounds(Vector3.zero, Vector3.one);
    private AnimationCurve curveField = AnimationCurve.Linear(0, 0, 1, 1);
    private Gradient gradientField;
    private GameObject objectField;
    private int popupIndex = 0;
    private string[] popupOptions = { "Option 1", "Option 2", "Option 3" };
    private float sliderValue = 0.5f;
    private float minMaxSliderMin = 0.2f;
    private float minMaxSliderMax = 0.8f;
    private float knobValue = 0.5f;
    private bool foldoutGroup = true;

    [MenuItem("Tools/EditorGUILayout Exhaustive Overview")]
    public static void ShowWindow()
    {
        GetWindow<EditorGUILayoutExhaustiveWindow>("GUILayout Exhaustive Overview");
    }

    private void OnGUI()
    {
        // Début du ScrollView
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.LabelField("EditorGUILayout - Exhaustive Overview", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // Basic Fields
        EditorGUILayout.LabelField("Basic Input Fields", EditorStyles.boldLabel);
        textField = EditorGUILayout.TextField("TextField", textField);
        intField = EditorGUILayout.IntField("IntField", intField);
        floatField = EditorGUILayout.FloatField("FloatField", floatField);
        toggleField = EditorGUILayout.Toggle("Toggle", toggleField);
        GUILayout.Space(10);

        // Advanced Fields
        EditorGUILayout.LabelField("Advanced Fields", EditorStyles.boldLabel);
        colorField = EditorGUILayout.ColorField("ColorField", colorField);
        vector2Field = EditorGUILayout.Vector2Field("Vector2Field", vector2Field);
        vector3Field = EditorGUILayout.Vector3Field("Vector3Field", vector3Field);
        vector4Field = EditorGUILayout.Vector4Field("Vector4Field", vector4Field);
        rectField = EditorGUILayout.RectField("RectField", rectField);
        boundsField = EditorGUILayout.BoundsField("BoundsField", boundsField);
        curveField = EditorGUILayout.CurveField("CurveField", curveField);
        gradientField = EditorGUILayout.GradientField("GradientField", gradientField);
        GUILayout.Space(10);

        // Object Field
        EditorGUILayout.LabelField("Object Field", EditorStyles.boldLabel);
        objectField = (GameObject)EditorGUILayout.ObjectField("ObjectField", objectField, typeof(GameObject), true);
        GUILayout.Space(10);

        // Popup and Enum
        EditorGUILayout.LabelField("Popup & Enums", EditorStyles.boldLabel);
        popupIndex = EditorGUILayout.Popup("Popup", popupIndex, popupOptions);
        GUILayout.Space(10);

        // Slider
        EditorGUILayout.LabelField("Sliders", EditorStyles.boldLabel);
        sliderValue = EditorGUILayout.Slider("Slider", sliderValue, 0f, 1f);
        EditorGUILayout.MinMaxSlider("MinMaxSlider", ref minMaxSliderMin, ref minMaxSliderMax, 0f, 1f);
        GUILayout.Label($"Min: {minMaxSliderMin:F2}, Max: {minMaxSliderMax:F2}");
        GUILayout.Space(10);
        
        // Knob
        EditorGUILayout.LabelField("Knob Control", EditorStyles.boldLabel);
        knobValue = EditorGUILayout.Knob(
            new Vector2(75, 75),
            knobValue,
            0f,
            1f,
            "Knob",
            Color.cyan,
            Color.gray,
            true);
        GUILayout.Space(10);

        // Foldout
        foldoutGroup = EditorGUILayout.Foldout(foldoutGroup, "Foldout Group");
        if (foldoutGroup)
        {
            EditorGUILayout.LabelField("Content inside the foldout!");
        }
        GUILayout.Space(10);

        // Horizontal & Vertical Layout
        EditorGUILayout.LabelField("Horizontal & Vertical Layouts", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Button("Button 1", GUILayout.Width(100));
        GUILayout.Button("Button 2", GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        GUILayout.Button("Vertical Button 1");
        GUILayout.Button("Vertical Button 2");
        EditorGUILayout.EndVertical();
        GUILayout.Space(10);

        // Help Box
        EditorGUILayout.LabelField("HelpBox", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("This is an informational HelpBox.", MessageType.Info);
        EditorGUILayout.HelpBox("This is a warning HelpBox.", MessageType.Warning);
        EditorGUILayout.HelpBox("This is an error HelpBox.", MessageType.Error);
        GUILayout.Space(10);

        // Separator
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        GUILayout.Space(10);

        // Debug Buttons
        EditorGUILayout.LabelField("Debug Buttons", EditorStyles.boldLabel);
        if (GUILayout.Button("Log Info"))
        {
            Debug.Log("Info logged!");
        }
        if (GUILayout.Button("Log Warning"))
        {
            Debug.LogWarning("Warning logged!");
        }
        if (GUILayout.Button("Log Error"))
        {
            Debug.LogError("Error logged!");
        }

        // Fin du ScrollView
        EditorGUILayout.EndScrollView();
    }
}

