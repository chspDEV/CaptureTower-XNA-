using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DOJO_ESTUDOS
{
    public enum IAState
    {
        Idle,
        MoveLeft,
        MoveRight,
        MoveFront,
        MoveBack,
        Patrol,
        Attacking,
        Dead
    }

    public class IA
    {
        public Vector3 Position { get; private set; }
        private Matrix Orientation = Matrix.Identity;
        private float scale;
        private Model model, projectileModel;
        public string name;

        public Vector3 myColor; //Usando assim por enquanto!

        private int Health;
        private int Damage;
        private IAState State;
        private float speed = 25f;
        private float moveDistance = .05f;
        private float searchDistance = 100f;
        
        private float attackCooldown = 1f; 
        private float attackTimer = 0;

        private int score = 0;

        private float randomStateTimer = 0; //DEBUG 
        public float randomStateTimerMax;
        private Random random = new Random(Guid.NewGuid().GetHashCode()); 

        private List<Projectile> projectiles = new List<Projectile>();  
        private IA target;
        private List<Tower> towers;

        public IA(Model model, Model projectileModel, Vector3 startPosition, int initialHealth, int damage, float scale = 1.0f)
        {
            this.projectileModel = projectileModel;
            this.model = model;
            Position = startPosition;
            Health = initialHealth;
            Damage = damage;
            State = IAState.Idle;
            this.scale = scale;


            randomStateTimerMax = Math.Max(1f, 1f * (float)random.NextDouble() * random.Next(1,2));

            //decidindo nome
            name = GameManager.Instance.GetRandomName(random);

            // decidindo cor
            myColor = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());

        }

        private void StateLogic(GameTime gameTime)
        {
           
            switch (State)
            {
                case IAState.Idle:
                    // nada
                    break;
                case IAState.MoveLeft:
                    Move(new Vector3(-moveDistance, 0, 0));
                    FindNearestTarget(); //DEBUG so por enquanto dps tirar
                    break;
                case IAState.MoveRight:
                    Move(new Vector3(moveDistance, 0, 0));
                    FindNearestTarget(); //DEBUG so por enquanto dps tirar
                    break;
                case IAState.MoveFront:
                    Move(new Vector3(0, 0, moveDistance));
                    FindNearestTarget(); //DEBUG so por enquanto dps tirar
                    break;
                case IAState.MoveBack:
                    Move(new Vector3(0, 0, -moveDistance));
                    FindNearestTarget(); //DEBUG so por enquanto dps tirar
                    break;
                case IAState.Attacking:
                    FindNearestTarget(); //DEBUG so por enquanto dps tirar
                    Attack(gameTime);
                    break;
                case IAState.Patrol:
                    FindNearestTarget();
                    break;
                case IAState.Dead:
                    
                    if (Position.Y > -0.5f)
                    {
                        Move(new Vector3(0, -moveDistance, 0));
                    }

                    break;
            }
        }

        // Métodos de Debug
        private void randomStateDebugger()
        {
            // Escolhe aleatoriamente um novo estado para a IA
            Array values = Enum.GetValues(typeof(IAState));
            IAState newState = (IAState)values.GetValue(random.Next(values.Length));

            // Definir um novo estado, mas evitar que a IA mude para o estado "Dead" aleatoriamente
            if (newState != IAState.Dead)
            {
                State = newState;
            }
        }

        private void Move(Vector3 direction)
        {
            Vector3 deslocamento = direction * speed * 0.1f;
            Vector3 posicaoFutura = Position + deslocamento;

            // X
            if (posicaoFutura.X < 0 || posicaoFutura.X > GameManager.Instance.mapWidth)
                deslocamento.X = 0; 

            // Z
            if (posicaoFutura.Z < 0 || posicaoFutura.Z > GameManager.Instance.mapHeight)
                deslocamento.Z = 0; 


            Position += deslocamento;
        }

        private void Attack(GameTime gameTime)
        {
            attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (attackTimer >= attackCooldown)
            {
                ShootProjectile();
                attackTimer = 0;
            }
        }

        private void ShootProjectile()
        {
            if (target != null) // SE EXISTE UM ALVO
            {
                // Cria um novo projetil
                var projectile = new Projectile(Position, target.Position, Damage, projectileModel);
                projectiles.Add(projectile);

            }
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            score--;
            UIManager.Instance.UpdateRanking();
            if (Health <= 0)
            {
                OnDefeated();
            }
        }

        private void UpdateProjectiles(GameTime gameTime)
        {
            foreach (var projectile in projectiles)
            {
                if (projectile.IsActive)
                {
                    projectile.Update(gameTime);

                    // Verifica se o projetil acertou
                    if (target != null && projectile.CheckCollision(this))
                    {
                        target.TakeDamage(projectile.Damage);
                        score++;
                        UIManager.Instance.UpdateRanking();
                        projectile.Deactivate(); // Desativa o projetil apos a colisão

                        //Matei
                        if (target.State == IAState.Dead) { score += 5; UIManager.Instance.UpdateRanking(); }
                    }
                }
            }

           projectiles.RemoveAll(p => !p.IsActive);
        }

        private void OnDefeated()
        {
            score -= 10;
            State = IAState.Dead;
            target = null;
            GameManager.Instance.CheckWin();
        }

        private void LookAtTarget()
        {
            if (target != null)
            {
                Vector3 directionToTarget = Vector3.Normalize(target.Position - Position);

                float yaw = (float)Math.Atan2(directionToTarget.X, directionToTarget.Z);

                Orientation = Matrix.CreateRotationY(yaw);
            }
        }

        // Métodos Públicos para Controlar as Ações da IA Externamente
        public void SetState(IAState newState)
        {
            if (State != IAState.Dead)
                State = newState;
        }

        public void FindNearestTarget()
        {
            float detectionRadius = searchDistance;
            float closestDistance = float.MaxValue;
            IA closestTarget = null;

            Vector3[] directions = {
            Vector3.Forward, Vector3.Backward,
            Vector3.Left, Vector3.Right,
            Vector3.Up, Vector3.Down
            };

            foreach (var direction in directions)
            {
                Ray ray = new Ray(Position, direction);
                float? hitDistance = null;
                IA hitTarget = null;

                foreach (var otherIA in GameManager.Instance.ias)
                {
                    if (otherIA == this || otherIA.State == IAState.Dead) continue;

                    BoundingSphere targetBounds = new BoundingSphere(otherIA.Position, 1f); 

                    // Testa se o Ray colide com o alvo
                    float? distance = ray.Intersects(targetBounds); 
                    if (distance.HasValue && distance.Value < detectionRadius)
                    {
                        if (hitDistance == null || distance.Value < hitDistance)
                        {
                            hitDistance = distance.Value;
                            hitTarget = otherIA;
                        }
                    }
                }

                if (hitTarget != null && hitDistance < closestDistance)
                {
                    closestDistance = hitDistance.Value;
                    closestTarget = hitTarget;
                }
            }

            target = closestTarget;
        }

        public void StartAttack()
        {
            if (State != IAState.Dead)
                State = IAState.Attacking;
        }

        public void StopAttack()
        {
            if (State == IAState.Attacking)
                State = IAState.Idle;
        }

        // Métodos de GET
        public int GetHealth() { return Health; }

        public int GetScore() { return score; }

        public IAState GetState() { return State; }

        public List<Projectile> GetProjectiles() { return projectiles;}

        public float GetScale() { return scale; }

        public void CaptureTower(Tower newTower)
        {
            score += 10;
            towers.Add(newTower);
        }

        // Draw e Update

        public void Update(GameTime gameTime)
        {
            if (GameManager.Instance.HasWinner) return;

            //debug PARA TROCAR DE ESTADOS POR ENQUANTO!!!

            randomStateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;


            if (randomStateTimer >= randomStateTimerMax && State != IAState.Dead)
            {
                randomStateDebugger();
                randomStateTimer = 0;
            }

            //Logica dos estados
            StateLogic(gameTime);

            //Olha para o alvo atual
            LookAtTarget();

            //Atualiza os projeteis
            UpdateProjectiles(gameTime);
        }

        public void Draw(Matrix view, Matrix projection)
        {
            Matrix world = Orientation * Matrix.CreateScale(scale) * Matrix.CreateTranslation(Position);

            if (model == null) { return; }

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                    effect.DiffuseColor = myColor; // Cor aleatoria
                }
                mesh.Draw();
            }

            // DESENHANDO PROJETEIS
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(view, projection);
            }

        }
    }


}
