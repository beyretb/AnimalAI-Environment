using System.Collections.Generic;
using UnityEngine;

namespace UnityEngineExtensions
{

    public static class TransformExtensions
    {
        public static List<GameObject> FindChildrenWithTag(this Transform parent, string tag)
        {
            List<GameObject> taggedChildren = new List<GameObject>();
            foreach (Transform child in parent)
            {
                if (child.CompareTag(tag))
                {
                    taggedChildren.Add(child.gameObject);
                }
            }
            return taggedChildren;
        }

        public static GameObject FindChildWithTag(this Transform parent, string tag)
        {
            foreach (Transform child in parent)
            {
                if (child.CompareTag(tag))
                {
                    return child.gameObject;
                }
            }
            return new GameObject();
        }

    }

    public static class GameObjectExtensions
    {
        public static Bounds GetBoundsWithChildren(this GameObject gameObj)
        {
            Bounds bound = new Bounds();
            bool boundFound = false;

            foreach (Collider coll in gameObj.GetComponents<Collider>())
            {
                if (boundFound)
                {
                    bound.Encapsulate(coll.bounds);
                }
                else
                {
                    bound = coll.bounds;
                    boundFound = true;
                }
            }

            foreach (Transform child in gameObj.transform)
            {
                if (child.childCount > 0)
                {
                    if (boundFound)
                    {
                        bound.Encapsulate(child.gameObject.GetBoundsWithChildren());
                    }
                    else
                    {
                        bound = child.gameObject.GetBoundsWithChildren(); ;
                        boundFound = true;
                    }
                }

                foreach (Collider coll in child.GetComponents<Collider>())
                {
                    if (boundFound)
                    {
                        bound.Encapsulate(coll.bounds);
                    }
                    else
                    {
                        bound = coll.bounds;
                        boundFound = true;
                    }
                }
            }
            return bound;
        }

        public static void SetLayer(this GameObject gameObj, int n)
        {
            gameObj.layer = n;
            foreach (Transform child in gameObj.transform)
            {
                child.gameObject.layer = n;
            }
        }
    }
}
