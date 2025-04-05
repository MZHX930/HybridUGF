using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevScript
{
    public class ScrollGroundGrid : MonoBehaviour
    {
        public SpriteRenderer[] LayerSRArray;

        public float GridHeight { get; private set; } = 0;
        public float GridWidth { get; private set; } = 0;

        public void Set(Sprite[] layerSprites)
        {
            for (int i = 0; i < LayerSRArray.Length; i++)
            {
                LayerSRArray[i].sprite = layerSprites[i];
                if (layerSprites[i] == null)
                {
                    GridHeight = 0;
                    GridWidth = 0;
                    LayerSRArray[i].transform.localPosition = Vector3.zero;
                }
                else
                {
                    //调整坐标
                    GridHeight = layerSprites[i].rect.height / Constant.GameLogic.SceneSpritePixelsPerUnit;
                    GridWidth = layerSprites[i].rect.width / Constant.GameLogic.SceneSpritePixelsPerUnit;
                    LayerSRArray[i].transform.localPosition = new Vector3(0, GridHeight / 2, 0);
                }
            }
        }

        public void SetLocalPosY(float posY)
        {
            transform.localPosition = new Vector3(0, posY, 0);
        }

        public float GetLocalPosY()
        {
            return transform.localPosition.y;
        }
    }
}