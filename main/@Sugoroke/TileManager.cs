using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileManager : MonoBehaviour
{
    private const string SHDER_PROPERTY_TEX_1 = "_Tex_1";
    private const string SHDER_PROPERTY_TEX_2 = "_Tex_2";
    private const float JUMP_TIME = 0.07f;

    [SerializeField] private List<GameObject> _tiles = new();
    [SerializeField] private Material _roadMaterial;
    [SerializeField] private List<Texture> _setTextures;
    [SerializeField] private Texture _defaultTextures;
    [SerializeField] private Shader _tileChange;

    private List<Material> materials = new();
    private int _changedCount;

    public List<GameObject> Tiles => _tiles;

    private void Awake()
    {
        SettingMaterialTile();
    }

    /// <summary>
    /// タイルObjにマテリアルを設定する
    /// </summary>
    private void SettingMaterialTile()
    {
        foreach (var tile in _tiles)
        {
            var renderer = tile.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                Material newMat = new Material(_tileChange);
                newMat.SetTexture(SHDER_PROPERTY_TEX_1, _defaultTextures);
                renderer.material = newMat;

                materials.Add(renderer.material);
            }
            else
            {
                Debug.LogWarning($"{tile.name} に Renderer または Material が設定されていません。");
                materials.Add(null); // nullで埋めておくか、スキップも可
            }
        }
    }

    private void OnDisable()
    {
        RoadMoving(false);
    }

    /// <summary>
    /// 中心FloorのMaterialをPieceの場所により変化
    /// </summary>
    public Tween ChangeRoadVisual(float time, int walkCount)
    {
        bool isEvenStep = walkCount % 2 == 0;
        float fromValue = isEvenStep ? 1f : 0f;
        float toValue = 1f - fromValue;

        return DOVirtual.Float(
        from: fromValue,
        to: toValue,
        duration: time,
            onVirtualUpdate: (tweenValue) =>
            {
                _roadMaterial.SetFloat("_ChangeValue", tweenValue);
            }).OnStart(() =>
            {
                ApplyRoadMaterialChange();
            });
    }

    /// <summary>
    /// マテリアルの変更処理
    /// _roadMaterialを適宜変更する
    /// </summary>
    private void ApplyRoadMaterialChange()
    {
        _changedCount++;

        int matIndex = _changedCount % materials.Count;
        Material tileMat = materials[matIndex];
        float tileValue = tileMat.GetFloat("_ChangeValue");

        string tileTexProp = tileValue == 0f ? SHDER_PROPERTY_TEX_1 : SHDER_PROPERTY_TEX_2;
        Texture newTex = tileMat.GetTexture(tileTexProp);

        string targetTexProp = (_changedCount % 2 == 0) ? SHDER_PROPERTY_TEX_1 : SHDER_PROPERTY_TEX_2;
        _roadMaterial.SetTexture(targetTexProp, newTex);
    }

    /// <summary>
    /// タイルの更新
    /// </summary>
    /// <param name="number">進行数</param>
    public void UpdateTiles()
    {
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < _tiles.Count; i++)
        {

            Transform _moveTrans = _tiles[i].transform;
            Vector3 setPos = _moveTrans.position;

            sequence.Append(_moveTrans.transform.DOJump(setPos, .7f, 1, JUMP_TIME));
            sequence.Join(ChangeTile(JUMP_TIME, materials[i]));
        }

        sequence.Play();
    }

    /// <summary>
    /// マテリアルのテクスチャ変更
    /// </summary>
    /// <param name="time"></param>
    /// <param name="mate"></param>
    /// <returns></returns>
    public Tween ChangeTile(float time, Material mate)
    {
        float to = 0;

        string propertyName = "";
        var value = mate.GetFloat("_ChangeValue");

        if (value == 0)
        {
            propertyName = SHDER_PROPERTY_TEX_2;
            to = 1;
        }
        else
        {
            propertyName = SHDER_PROPERTY_TEX_1;
            to = 0;
        }

        int rnd = Random.Range(0, _setTextures.Count);
        mate.SetTexture(propertyName, _setTextures[rnd]);

        return DOVirtual.Float(
            from: value,
            to: to,
            duration: time,
            onVirtualUpdate: (tweenValue) =>
            {
                mate.SetFloat("_ChangeValue", tweenValue);
            });
    }

    /// <summary>
    /// 中心の床を動作有無
    /// </summary>
    public void RoadMoving(bool isMoving)
    {
        Vector2 speed = (isMoving) ? new Vector2(0f, -2f) : Vector2.zero;
        _roadMaterial.SetVector("_Speed", speed);
    }
}


