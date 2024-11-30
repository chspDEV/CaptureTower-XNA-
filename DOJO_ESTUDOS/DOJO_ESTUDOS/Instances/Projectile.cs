using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DOJO_ESTUDOS
{
    public class Projectile
    {
        public Vector3 Position { get; private set; }
        public Vector3 Direction { get; private set; }
        public int Damage { get; private set; }
        public bool IsActive { get; private set; }

        private float speed = 10f;
        //scale = .35f;
        private float scale = 5f;
        private Model model;  // O modelo 3D do projétil

        private float lifeTime = 10f;
        private float count;

        public Projectile(Vector3 startPosition, Vector3 targetPosition, int damage, Model projectileModel)
        {
            Direction = Vector3.Normalize(targetPosition - startPosition);

            // Evitar spawn dentro do alvo
            float collisionDistance = 1.0f; 
            if (Vector3.Distance(startPosition, targetPosition) < collisionDistance)
            {
                Position = startPosition - Direction * collisionDistance;
            }
            else
            {
                Position = startPosition;
            }

            //GameManager.Instance.camera.ChangePosition(Position);

            Damage = damage;
            IsActive = true;
            model = projectileModel;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            Position += Direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            count += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (count >= lifeTime) Deactivate();
        }

        // Método para desativar o projétil em caso de colisão
        public void Deactivate()
        {
            IsActive = false;
        }

        public bool CheckCollision()
        {
            float collisionDistance = 1.5f; 
            float closestDistance = float.MaxValue;
            IA closestTarget = null;

            foreach (IA ia in GameManager.Instance.ias)
            {
                float distance = Vector3.Distance(Position, ia.Position);

                // Atualiza o alvo mais próximo
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = ia;
                }
            }

            if (closestTarget != null && closestDistance <= collisionDistance)
            {
                return true;
            }

            return false;
        }

        // Método para desenhar o projétil
        public void Draw(Matrix view, Matrix projection)
        {
            if (model == null || !IsActive) { return; }

            //criando uma escala
            Matrix scaleMatrix = Matrix.CreateScale(scale, scale, scale);

            // Matriz de mundo para posicionar e alinhar o projétil na direção certa
            Matrix world = Matrix.CreateWorld(Position, Direction, Vector3.Up);

            //multiplicando tudo junto
            world = scaleMatrix * world;

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}
