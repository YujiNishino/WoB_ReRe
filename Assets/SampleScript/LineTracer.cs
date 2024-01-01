using System;
using UnityEngine;

public class LineTracer : MonoBehaviour
{
    [SerializeField] private LineDraw linedraw;

    private Vector3[] positions;

    private bool isMove;
    private Vector3 target;
    private int index = 0;
    private float speed = 5.0f;

    private void Start()
    {
        isMove = false;
    }
    private void OnMouseDown()
    {
        index = 0;
        linedraw.StartPosition(transform.position);
    }

    private void OnMouseDrag()
    {
        linedraw.AddPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private void OnMouseUp()
    {
        // マウスドラッグで描画された複数のポイントを取得
        positions = new Vector3[linedraw.lineRenderer.positionCount];
        linedraw.lineRenderer.GetPositions(positions);
        // 移動を許可
        isMove = true;
    }

    private void Update()
    {
        if (!isMove) return;

        //
        if (Vector2.Distance(target, transform.position) <= 0.1f)
        {
            // indexがポジション数以下
            if (index >= positions.Length - 1) 
            {
                // 動かない
                isMove = false;
            }
            // 線の終点
            if (index != positions.Length - 1)
            {
                index++;
                target = positions[index];
            }
        }
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        Debug.Log("MoveToStart!!");


    }
}