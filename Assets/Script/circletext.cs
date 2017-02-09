using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class circletext : BaseMeshEffect {

  
    public int radius = 50;
    public float spaceCoff = 1f;

    Text _myText;

    #region implemented absract members
    public override void ModifyMesh(VertexHelper vh) {
        //text component
        _myText = GetComponent<Text>();
        TextGenerator _tg = _myText.cachedTextGenerator;
        List<UIVertex> stream = new List<UIVertex>();
        vh.GetUIVertexStream(stream);

        float parameter = Mathf.PI * radius * 2;
        float weight = _myText.fontSize / parameter * spaceCoff;
        float radStep = Mathf.PI * 2 * weight;
        float charOffset = _tg.characterCountVisible / 2f - 0.5f;

        for (int i = 0; i < _tg.characterCountVisible; i++)
        {

            var lt = stream[i * 6];
            var rt = stream[i * 6 + 1];
            var rb = stream[i * 6 + 2];
            var lb = stream[i * 6 + 4];

            Vector3 center = Vector3.Lerp(lb.position, rt.position, 0.5f);
            Matrix4x4 move = Matrix4x4.TRS(center * -1, Quaternion.identity, Vector3.one);
            float rad = Mathf.PI / 2 + (charOffset - i) * radStep;
            Vector3 pos = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;

            Quaternion rotation = Quaternion.Euler(0, 0, rad * 180 / Mathf.PI - 90);
            Matrix4x4 rotate = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
            Matrix4x4 place = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
            Matrix4x4 transform = place * rotate * move;

            lb.position = transform.MultiplyPoint(lb.position);
            lt.position = transform.MultiplyPoint(lt.position);
            rt.position = transform.MultiplyPoint(rt.position);
            rb.position = transform.MultiplyPoint(rb.position);

            stream[i * 6] = lt;
            stream[i * 6 + 1] = rt;
            stream[i * 6 + 2] = rb;
            stream[i * 6 + 3] = rb;
            stream[i * 6 + 4] = lb;
            stream[i * 6 + 5] = lt;
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(stream);
    }
    #endregion
}
