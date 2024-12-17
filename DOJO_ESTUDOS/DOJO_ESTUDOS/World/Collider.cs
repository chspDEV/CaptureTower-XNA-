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
            collider = new BoundingBox(newPosition, newPosition + size);
        }

        public void SetupBoundingBox(Vector3 min, Vector3 max)
        {
            collider = new BoundingBox(min, max);
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