using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CurvedText : Text
{
	[field: SerializeField] public float Radius { get; set; } = 0.5f;
    [SerializeField] private float _scaleFactor = 100.0f;

	private float _circumference => 2.0f * Mathf.PI * Radius * _scaleFactor;

	#if UNITY_EDITOR
    protected override void OnValidate()
	{
		base.OnValidate();
		if(Radius <= 0.0f)
            Radius = 0.001f;
		if(_scaleFactor <= 0.0f)
            _scaleFactor = 0.001f;
	}
	#endif // UNITY_EDITOR

    protected override void OnPopulateMesh(VertexHelper vertexHelper)
	{	
		base.OnPopulateMesh(vertexHelper);

		List<UIVertex> stream = new List<UIVertex>();

		vertexHelper.GetUIVertexStream(stream);

		for (int i = 0; i < stream.Count; i++)
		{
			UIVertex vertex = stream[i];

			float percentCircumference = vertex.position.x / _circumference;
			Vector3 offset = Quaternion.Euler(0.0f,0.0f,-percentCircumference*360.0f)*Vector3.up;
			vertex.position = offset* Radius * _scaleFactor + offset*vertex.position.y;
			vertex.position += Vector3.down* Radius * _scaleFactor;

			stream[i] = vertex;
		}

		vertexHelper.AddUIVertexTriangleStream(stream);
	}
}
