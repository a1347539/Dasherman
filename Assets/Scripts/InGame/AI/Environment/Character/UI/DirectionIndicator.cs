using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.AI.Environment.Character
{
    public class DirectionIndicator : MonoBehaviour
    {
        private RectTransform rectTransform;

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, MapController.Instance.cellSize * 51.3f);
        }

        public void scaleIndicator(int direction, int distance)
        {
            // facing left: 0, facing up: 1, facing right: 2, facing down: 3
            if (direction == 0 || direction == 2)
            {
                rectTransform.rotation = Quaternion.Euler(
                        rectTransform.rotation.x,
                        rectTransform.rotation.y,
                        0
                        );
            }
            else if (direction == 1 || direction == 3)
            {
                rectTransform.rotation = Quaternion.Euler(
                        rectTransform.rotation.x,
                        rectTransform.rotation.y,
                        90
                        );
            }

            rectTransform.localScale = new Vector2(distance, rectTransform.localScale.y);

        }

        public void resetIndicator()
        {
            rectTransform.localScale = new Vector2(0, rectTransform.localScale.y);
        }
    }
}