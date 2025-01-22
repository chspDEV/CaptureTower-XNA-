using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DOJO_ESTUDOS
{
    public class Collider
    {
        public BoundingBox collider { get; protected set; }

        public Collider()
        {

        }

        public void UpdateBoundingBox(Vector3 newPosition)
        {
            Vector3 size = collider.Max - collider.Min;
            Vector3 adjustedPosition = new Vector3(newPosition.X, newPosition.Y + size.Y * 0.5f, newPosition.Z);
            collider = new BoundingBox(adjustedPosition, adjustedPosition + size);
        }

        public void SetupBoundingBox(Vector3 min, Vector3 max)
        {
            // tamanho real 
            Vector3 size = max - min;

            // ajuste para parte inferior do modelo (origem do modelo esta na parte central inferior)
            Vector3 adjustedMin = new Vector3(min.X, min.Y + size.Y * 0.5f, min.Z);
            Vector3 adjustedMax = new Vector3(max.X, max.Y + size.Y * 0.5f, max.Z);

            collider = new BoundingBox(adjustedMin, adjustedMax);
        }

        public bool Intersects(Collider other)
        {
            return collider.Intersects(other.collider);
        }
        
        public bool Intersects(BoundingSphere sphere)
        {
            return collider.Intersects(sphere);
        }

        public bool Intersects(Ray ray)
        {
            return ray.Intersects(collider).HasValue;
        }

    }
}