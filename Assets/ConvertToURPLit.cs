// Va en una carpeta llamada "Editor" (por ejemplo Assets/Editor/).
// Uso: selecciona materiales en el Project → Tools → Convertir materiales seleccionados a URP Lit.
using UnityEditor;
using UnityEngine;

public static class ConvertToURPLit
{
    // Nombres comunes que usan los shaders de assets para cada textura.
    static readonly string[] ColorTexNames  = { "_BaseMap", "_MainTex", "_Albedo", "_AlbedoMap", "_Diffuse", "_DiffuseMap", "_BaseColorMap", "_Texture" };
    static readonly string[] NormalTexNames = { "_BumpMap", "_NormalMap", "_Normal", "_NormalTex" };
    static readonly string[] ColorNames     = { "_BaseColor", "_Color", "_Tint", "_MainColor", "_AlbedoColor" };

    [MenuItem("Tools/Convertir materiales seleccionados a URP Lit")]
    static void Convert()
    {
        Shader lit = Shader.Find("Universal Render Pipeline/Lit");
        if (lit == null) { Debug.LogError("No encuentro URP/Lit. ¿El proyecto tiene URP instalado?"); return; }

        int count = 0;
        foreach (Object o in Selection.GetFiltered(typeof(Material), SelectionMode.Assets))
        {
            var m = (Material)o;
            if (m.shader == lit) continue;

            // 1. Leer lo que hay ANTES de cambiar el shader (después se pierde).
            Texture colorTex  = FindTex(m, ColorTexNames);
            Texture normalTex = FindTex(m, NormalTexNames);
            Color color = FindColor(m, ColorNames);
            Vector2 tiling = Vector2.one, offset = Vector2.zero;
            string tilingProp = FindName(m, ColorTexNames);
            if (tilingProp != null) { tiling = m.GetTextureScale(tilingProp); offset = m.GetTextureOffset(tilingProp); }

            // 2. Cambiar shader y reasignar.
            Undo.RecordObject(m, "Convertir a URP Lit");
            m.shader = lit;
            if (colorTex != null) { m.SetTexture("_BaseMap", colorTex); m.SetTextureScale("_BaseMap", tiling); m.SetTextureOffset("_BaseMap", offset); }
            m.SetColor("_BaseColor", color);
            if (normalTex != null) { m.SetTexture("_BumpMap", normalTex); m.EnableKeyword("_NORMALMAP"); }

            EditorUtility.SetDirty(m);
            count++;
            Debug.Log($"Convertido: {m.name} (color: {(colorTex ? colorTex.name : "ninguna")}, normal: {(normalTex ? normalTex.name : "ninguna")})", m);
        }
        AssetDatabase.SaveAssets();
        Debug.Log($"Listo: {count} materiales convertidos a URP Lit.");
    }

    [MenuItem("Tools/Convertir materiales seleccionados a URP Lit", true)]
    static bool Validate() => Selection.GetFiltered(typeof(Material), SelectionMode.Assets).Length > 0;

    static string FindName(Material m, string[] names)
    {
        foreach (var n in names) if (m.HasProperty(n) && m.GetTexture(n) != null) return n;
        return null;
    }
    static Texture FindTex(Material m, string[] names)
    {
        var n = FindName(m, names);
        return n == null ? null : m.GetTexture(n);
    }
    static Color FindColor(Material m, string[] names)
    {
        foreach (var n in names) if (m.HasProperty(n)) return m.GetColor(n);
        return Color.white;
    }
}
