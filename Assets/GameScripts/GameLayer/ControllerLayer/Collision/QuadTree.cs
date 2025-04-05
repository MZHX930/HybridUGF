using UnityEngine;
using System.Collections.Generic;

namespace GameDevScript
{
    public class QuadTree
    {
        private Rect boundary;
        private int capacity;
        private List<GameObject> objects;
        private bool divided;

        private QuadTree northEast;
        private QuadTree northWest;
        private QuadTree southEast;
        private QuadTree southWest;

        public QuadTree(Rect boundary, int capacity)
        {
            this.boundary = boundary;
            this.capacity = capacity;
            this.objects = new List<GameObject>();
            this.divided = false;
        }

        private void Subdivide()
        {
            float x = boundary.x;
            float y = boundary.y;
            float w = boundary.width / 2;
            float h = boundary.height / 2;

            Rect ne = new Rect(x + w, y, w, h);
            northEast = new QuadTree(ne, capacity);

            Rect nw = new Rect(x, y, w, h);
            northWest = new QuadTree(nw, capacity);

            Rect se = new Rect(x + w, y + h, w, h);
            southEast = new QuadTree(se, capacity);

            Rect sw = new Rect(x, y + h, w, h);
            southWest = new QuadTree(sw, capacity);

            divided = true;
        }

        public bool Insert(UnitCollider collider)
        {
            // // AABBCollider collider = obj.GetComponent<AABBCollider>();

            // // 如果没有碰撞体组件，使用中心点
            // if (collider == null)
            // {
            //     Vector2 position = collider.transform.position;
            //     if (!boundary.Contains(position))
            //         return false;
            // }
            // else
            // {
            //     // 使用碰撞盒检查是否与边界相交
            //     Bounds bounds = collider.GetBounds();
            //     Rect colliderRect = new Rect(
            //         bounds.min.x,
            //         bounds.min.y,
            //         bounds.size.x,
            //         bounds.size.y
            //     );

            //     // 如果碰撞盒不与边界相交，则不插入
            //     if (!RectOverlaps(boundary, colliderRect))
            //         return false;
            // }

            // // 其余插入逻辑保持不变
            // if (objects.Count < capacity && !divided)
            // {
            //     objects.Add(collider.gameObject);
            //     return true;
            // }

            // if (!divided)
            //     Subdivide();

            // if (northEast.Insert(collider))
            //     return true;
            // if (northWest.Insert(collider))
            //     return true;
            // if (southEast.Insert(collider))
            //     return true;
            // if (southWest.Insert(collider))
            //     return true;

            return false;
        }

        public List<GameObject> Query(UnitCollider collider, List<GameObject> found)
        {
            if (found == null)
                found = new List<GameObject>();

            // if (!boundary.Overlaps(range))
            //     return found;

            // foreach (GameObject obj in objects)
            // {
            //     if (range.Contains(obj.transform.position))
            //         found.Add(obj);
            // }

            // if (divided)
            // {
            //     northEast.Query(range, found);
            //     northWest.Query(range, found);
            //     southEast.Query(range, found);
            //     southWest.Query(range, found);
            // }

            return found;
        }

        public void Clear()
        {
            objects.Clear();

            if (divided)
            {
                northEast.Clear();
                northWest.Clear();
                southEast.Clear();
                southWest.Clear();

                divided = false;
            }
        }
    }
}
