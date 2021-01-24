using UnityEngine;

[ExecuteInEditMode]
public class CurvedWorld : MonoBehaviour {

    public Vector3 Curvature = new Vector3(0, 0.5f, 0);
    public float Distance = 0;

    [Space]
    public float CurvatureScaleUnit = 1000f;
#if UNITY_EDITOR
    public bool IsRunInEditMode = false;

#endif
    private int _curvatureID;
    private int _distanceID;

    private void OnEnable()
    {
        _curvatureID = Shader.PropertyToID("_Curvature");
        _distanceID = Shader.PropertyToID("_Distance");
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!IsRunInEditMode && !Application.isPlaying)
        {
            Shader.SetGlobalVector(_curvatureID, Vector3.zero);
            Shader.SetGlobalFloat(_distanceID, 0);
            return;
        }
#endif
        Vector3 curvature = CurvatureScaleUnit == 0 ? Curvature : Curvature / CurvatureScaleUnit;

        Shader.SetGlobalVector(_curvatureID, curvature);
        Shader.SetGlobalFloat(_distanceID, Distance);
    }
}
