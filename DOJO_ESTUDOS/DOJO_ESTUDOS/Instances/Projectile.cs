using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DOJO_ESTUDOS
{
    public class Projectile: Collider
    {
        public Vector3 Position { get; private set; }
        public Vector3 Direction { get; private set; }
        public int Damage { get; private set; }
        public bool IsActive { get; private set; }
        
        private float speed = 35f;
        private float scale = .6f;
        private Model model; 

        private float lifeTime = 10f;
        private float count;
        public string identifier = " [b] ";

        public IA colliderIa = null;
        public IA pai;

        public Projectile(IA pai, Vector3 startPosition, Vector3 targetPosition, int damage, Model projectileModel)
        {
            this.pai = pai;

            //achando direcao
            Direction = Vector3.Normalize(targetPosition - startPosition);

            Position = startPosition + Direction;

            //criando colisor
            Vector3 scaleBox = new Vector3(scale, scale, scale);
            SetupBoundingBox(Position - scaleBox, Position + scaleBox);

            Damage = damage;
            IsActive = true;
            model = projectileModel;
        }


        public void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            Position += Direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            count +=(float)gameTime.ElapsedGameTime.TotalSeconds;

            if (count >= lifeTime) Deactivate();

            UpdateBoundingBox(Position);
            CheckCollision();
            AttackLogic();
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void AttackLogic()
        {
            if (colliderIa != null)
            {
                colliderIa.TakeDamage(Damage);
                pai.AddScore(1); //recompensa por acertar
                if (colliderIa.GetState() == IAState.Dead) pai.AddScore(5); //recompensa por matar
                UIManager.Instance.UpdateRanking();
                Deactivate(); 
            }
        }

        public void CheckCollision()
        {
            IA closestTarget = null;

            foreach (IA ia in GameManager.Instance.ias)
            {
                if (collider.Intersects(ia.collider) && ia != pai)
                {
                    closestTarget = ia;
                    colliderIa = ia;
                }
            }
        }

        // Método para desenhar o projétil
        public void Draw(Matrix view, Matrix projection)
        {
            if (model == null || !IsActive) { return; }

            //criando uma escala
            Matrix scaleMatrix = Matrix.CreateScale(scale, scale, scale);

            // Matriz de mundo para posicionar e alinhar o projétil na direção certa
            Matrix world = Matrix.CreateScale(scale) * Matrix.CreateWorld(Position, Direction, Vector3.Up);


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
