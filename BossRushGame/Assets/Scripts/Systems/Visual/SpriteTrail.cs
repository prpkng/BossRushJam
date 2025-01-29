namespace BRJ.Systems.Visual
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using PrimeTween;
    using UnityEngine;

    public class SpriteTrail : MonoBehaviour
    {
        public int objectPoolingCount = 100;
        public SpriteRenderer targetRenderer;

        public float emittingRate = 4;
        public GameObject trailPrefab;
        public TweenSettings fadeOutTween;

        private Queue<Transform> availableObjects;

        private async void Awake()
        {
            var op = InstantiateAsync(trailPrefab, objectPoolingCount, transform);
            await op;
            availableObjects = new Queue<Transform>(from obj in op.Result select obj.transform);
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => availableObjects != null);
            while (true)
            {
                if (availableObjects.Count == 0)
                {
                    Debug.LogError("Pooling object overflow, reduce the trail speed or increase the object pooling count");
                    yield return new WaitWhile(() => availableObjects.Count == 0);
                }
                var obj = availableObjects.Dequeue();
                obj.gameObject.SetActive(true);
                obj.transform.parent = null;
                obj.transform.position = transform.position;
                obj.transform.rotation = transform.rotation;
                obj.transform.localScale = transform.lossyScale;
                var spr = obj.GetComponent<SpriteRenderer>();
                spr.sprite = targetRenderer.sprite;
                Tween.Custom(
                    spr,
                    1,
                    0,
                    fadeOutTween,
                    (obj, f) =>
                    {
                        var clr = obj.color;
                        clr.a = f;
                        obj.color = clr;
                    }
                ).OnComplete(obj, target =>
                {
                    availableObjects.Enqueue(target);
                    target.parent = transform;
                    target.gameObject.SetActive(false);
                });

                yield return new WaitForSeconds(1f / emittingRate);
            }
        }
    }
}