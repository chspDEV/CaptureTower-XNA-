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
        Catch,
        MoveLeft,
        MoveRight,
        MoveFront,
        MoveBack,
        Idle,
        Attacking,
        Dead
    }

    public class IA: Collider
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
        
        private float attackCooldown = .5f; 
        private float attackTimer = 0;

        private int score = 0;

        private float randomStateTimer = 0; //DEBUG 
        public float randomStateTimerMax;
        private Random random = new Random(Guid.NewGuid().GetHashCode()); 

        private List<Projectile> projectiles = new List<Projectile>();  
        private IA target;

        private List<Tower> towers = new List<Tower>();

        public IA(Model model, Model projectileModel, Vector3 startPosition, int initialHealth, int damage, float scale = 1.0f)
        {
            

            this.projectileModel = projectileModel;
            this.model = model;
            Position = startPosition;
            Health = initialHealth;
            Damage = damage;
            State = IAState.Idle;
            this.scale = scale;

            //debug para trocar estados
            randomStateTimerMax = 2f;

            //criando colisor
            Vector3 scaleBox = new Vector3(scale * 100f, scale * 100f, scale * 100f);
            SetupBoundingBox(Position - scaleBox, Position + scaleBox);

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
                case IAState.Catch:
                    //COLOCAR PARA IR PRA TORRE MAIS PROXIMA
                    FindTower();
                    break;
                case IAState.Dead:

                    foreach (var t in towers)
                    {
                        t.owner = null;
                        t.myColor = Vector3.Zero;
                    }

                    towers.Clear();

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
            //IAState newState = (IAState)values.GetValue(random.Next(0,5));

            // Definir um novo estado, mas evitar que a IA mude para o estado "Dead" aleatoriamente
            if (newState != IAState.Dead)
            {
                State = newState;
            }
        }

        private void Move(Vector3 direction)
        {
            float offset = GameManager.Instance.maxDistanceToMove; 

            Vector3 deslocamento = direction * speed * 0.1f;
            Vector3 posicaoFutura = Position + deslocamento;

            // X
            if (posicaoFutura.X < -offset + scale || posicaoFutura.X > offset - scale)
            {
                deslocamento.X = 0; 
            }

            // Z
            if (posicaoFutura.Z < -offset + scale || posicaoFutura.Z > offset - scale)
            {
                deslocamento.Z = 0; 
            }

            Position += deslocamento;
        }

        private void Attack(GameTime gameTime)
        {
            
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
                var projectile = new Projectile(this, Position, target.Position, Damage, projectileModel);
                projectiles.Add(projectile);

            }
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            UIManager.Instance.UpdateRanking();
            if (Health <= 0)
            {
                OnDefeated();
            }
        }

        public void AddScore(int newScore) { score += newScore; }

        private void UpdateProjectiles(GameTime gameTime)
        {
            foreach (var projectile in projectiles)
            {
                if (projectile.IsActive)
                {
                    projectile.Update(gameTime);
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

        public void CaptureTower(Tower newTower, bool reward = true)
        {
            if (!towers.Contains(newTower)) // Evita capturar a mesma torre
            {
                if (reward) score += 10; 
                towers.Add(newTower);
            }
        }

        private bool FindTower()
        {

            Vector3[] directions = {
            Vector3.Forward, Vector3.Backward,
            Vector3.Left, Vector3.Right,
            new Vector3(1, 0, 1), new Vector3(-1, 0, 1),
            new Vector3(1, 0, -1), new Vector3(-1, 0, -1)
            };
    
            foreach (var direction in directions)
            {
                Ray ray = new Ray(Position, direction);

                foreach (Tower torre in GameManager.Instance.towers)
                {
                    if (this.towers.Contains(torre)) continue; // Evita verificar torres já capturadas

                    float? distance = ray.Intersects(torre.collider);

                    if (distance.HasValue && distance.Value < 100f)
                    {
                        torre.CheckNewOwner();
                        return true;
                    }
                }
            }

            return false;
        }


        #region Metodos externalizados em LUA

        // Métodos Estado
        public void SetState(IAState newState)
        {
            if (State != IAState.Dead)
                State = newState;
        }
        public void MoveLeft() { if (State != IAState.Dead) State = IAState.MoveLeft; }
        public void MoveRight() { if (State != IAState.Dead) State = IAState.MoveRight; }
        public void MoveBack() { if (State != IAState.Dead) State = IAState.MoveBack; }
        public void MoveFront() { if (State != IAState.Dead) State = IAState.MoveFront; }
        public void Catch() { if (State != IAState.Dead) State = IAState.Catch; }
        public void StartAttack() { if (State != IAState.Dead) State = IAState.Attacking;   }
        public void Idle() { if (State != IAState.Dead) State = IAState.Idle; }
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

        
        // Métodos de GET
        public int GetHealth() { return Health; }

        public int GetScore() { return score; }

        public int GetMaxHealth() { return 100; }

        #endregion

        public IAState GetState() { return State; }

        public List<Projectile> GetProjectiles() { return projectiles;}

        public List<Tower> GetTowers() { return towers; }

        public float GetScale() { return scale; }

        

        // Draw e Update

        public void Update(GameTime gameTime)
        {
            if (GameManager.Instance.HasWinner) return;

            //debug PARA TROCAR DE ESTADOS POR ENQUANTO!!!

            randomStateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Debug
            if (randomStateTimer >= randomStateTimerMax && State != IAState.Dead)
            {
                randomStateDebugger();
                randomStateTimer = 0;
            }

            //Logica para ataques
            if (attackTimer < attackCooldown)
            {
                attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            

            //Logica dos estados
            StateLogic(gameTime);

            //Atualiza colisor
            UpdateBoundingBox(Position);

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
