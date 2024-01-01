using UnityEngine;

/// <summary>
/// 線描画クラス
/// </summary>
public class LineDraw : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private int posCount = 0;       // 描画ポイント数
    private float interval = 0.5f;  // 描画精度

    /// <summary>
    /// 線描オブジェクトを取得
    /// </summary>
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// ポジション開始点
    /// </summary>
    /// <param name="pos"></param>
    public void StartPosition(Vector2 pos)
    {   
        // 描画ポイント数リセット
        posCount = 0;
        // インデックスを１
        lineRenderer.positionCount = 1;
        // 呼ばれたポジションを開始点とする
        lineRenderer.SetPosition(0, pos);
    }

    /// <summary>
    /// ポジション追加
    /// </summary>
    /// <param name="pos"></param>
    public void AddPosition(Vector2 pos)
    {
        // 描画精度チェック
        if (!PosCheck(pos))
        {
            return; // 直前のポジションが近い場合は描画しない
        }
        // 直線描画対応（Index1,2のみ）
        if (posCount < 2) 
        {
            posCount++;
        }
        lineRenderer.positionCount = posCount;
        lineRenderer.SetPosition(posCount - 1, pos);
    }

    /// <summary>
    /// 描画精度のチェック
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private bool PosCheck(Vector2 pos)
    {
        // 未描画は対象外
        if (posCount == 0) return true;
        // 直前のIndexのポジションと差をintervalでチェック
        float distance = Vector2.Distance(lineRenderer.GetPosition(posCount - 1), pos);
        if (distance > interval)
        {
            return true;    // interval以上：描画する
        }
        return false;   // interval以下：描画しない
    }
}