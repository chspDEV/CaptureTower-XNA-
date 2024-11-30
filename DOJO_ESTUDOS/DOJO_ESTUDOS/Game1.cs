using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace DOJO_ESTUDOS
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Model iaModel, projectileModel, towerModel;
        private List<IA> ias = new List<IA>();
        private List<Tower> towers = new List<Tower>();
        private Camera camera;
        private Ground ground;

        private GameManager gm;
        private UIManager ui;

        private SpriteFont debugFont;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            // Configurando o tamanho inicial da janela
            graphics.PreferredBackBufferWidth = 1280; // Largura
            graphics.PreferredBackBufferHeight = 720; // Altura
            graphics.ApplyChanges(); // Aplicar as mudanças
        }

        protected override void Initialize()
        {
            camera = new Camera(GraphicsDevice);
            gm = new GameManager();
            gm.camera = camera;
            

            base.Initialize();

        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Carregando modelos
            iaModel = Content.Load<Model>(@"Modelos\IAModel");
            projectileModel = Content.Load<Model>(@"Modelos\projectileModel");
            towerModel = Content.Load<Model>(@"Modelos\TorreModel");

            // Carregando fontes
            debugFont = Content.Load<SpriteFont>(@"Fonts\DebugFont");
            ui = new UIManager(debugFont);

            int players = 27;

            ui.ranking = new List<String>(players);

            ground = new Ground(GraphicsDevice, Color.Green, players * 5.5f);

            
            int offsetSpawn = Math.Max(5, (int)(10 * Math.Sqrt(players)));

            int columns = (int)Math.Ceiling(Math.Sqrt(players)); // Número de colunas RAIZ QUADRADA DOS JOGADORES (Arredondado pra cima)
            int rows = (int)Math.Ceiling((float)players / columns); // Número de linhas Jogadores dividido por colunas (Arredondado pra cima)

            // tamanhos grid
            float gridWidth = (columns - 1) * offsetSpawn;
            float gridHeight = (rows - 1) * offsetSpawn;

            for (int c = 0; c < columns; c++) // colunas
            {
                for (int r = 0; r < rows; r++) // linhas
                {

                    if (ias.Count >= players) return;

                    Vector3 startPosition = new Vector3(
                        c * offsetSpawn - gridWidth / 2,       
                        1,
                        r * offsetSpawn - gridHeight / 2 
                    );

                    IA ia = new IA(iaModel, projectileModel, startPosition, initialHealth: 100, damage: 10, scale: .01f);

                    startPosition.Z -= 15f;
                    Tower torre = new Tower(towerModel, startPosition, scale: 2f);

                   
                    ias.Add(ia);
                    towers.Add(torre);

                   
                    UpdateIAList();
                }
            }

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            camera.Update();

            foreach (var ia in ias)
            {
                ia.Update(gameTime);
            }

            foreach (var t in towers)
            {
                t.Update(gameTime);
            }

            ui.Update(gameTime);
            gm.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkBlue);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // CHAO
            ground.Draw(GraphicsDevice, camera.ViewMatrix, camera.ProjectionMatrix);

            // IAS
            foreach (var ia in ias)
            {
                ia.Draw(camera.ViewMatrix, camera.ProjectionMatrix);
            }

            // TORRES
            foreach (var t in towers)
            {
                t.Draw(camera.ViewMatrix, camera.ProjectionMatrix);
            }

            // INTERFACE
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            // INTERFACE
            ui.Draw(spriteBatch, GraphicsDevice);

            spriteBatch.End();


            base.Draw(gameTime);
        }

        public void UpdateIAList()
        {
            GameManager.Instance.ias = ias;
        }

    }
}