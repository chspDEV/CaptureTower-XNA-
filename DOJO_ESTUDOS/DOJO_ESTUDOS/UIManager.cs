using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DOJO_ESTUDOS
{
    class UIManager
    {
        public static UIManager Instance;

        public List<string> debugTexts = new List<string>(5);
        public List<string> ranking;

        private SpriteFont debugFont;
        
        float elapsedTime = 0;
        int frameCount = 0;
        float fps = 0;

        public UIManager(SpriteFont debugFont)
        {
            if (Instance != null)
            {
                throw new InvalidOperationException("DebugManager já foi instanciado.");
            }

            Instance = this;
            this.debugFont = debugFont;
        }

        public void SetText(List<string> list,int index, string text)
        {
            if (list == null) return;

            while (list.Count <= index)
            {
                list.Add("");
            }

            list[index] = text;
        }

        void FPS(GameTime gameTime)
        {
            frameCount++;
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsedTime >= 1.0f)
            {
                fps = frameCount / elapsedTime;
                SetText(debugTexts, 0, "FPS: " + fps.ToString("0.00"));
                frameCount = 0;
                elapsedTime = 0;
            }
        }

        public void Update(GameTime gt)
        {
            FPS(gt);

            for (var i = 0; i < GameManager.Instance.ias.Count; i++)
            {
                IA currentIA = GameManager.Instance.ias[i];

                string txt;

                txt = "HP[" + currentIA.GetHealth() + "/100] " + currentIA.name + " SCORE: " + currentIA.GetScore();

                if (i < 5) { txt = (i+1).ToString() + " HP[" + currentIA.GetHealth() + "/100] " + currentIA.name + " SCORE: " + currentIA.GetScore(); }
                 
                UIManager.Instance.SetText(ranking,i, txt);
            }
        }

        public void UpdateRanking()
        {
            // RANKING
            var ias = GameManager.Instance.ias;
            // Ordena as IAs pelo score em ordem decrescente
            ias.Sort((ia1, ia2) => ia2.GetScore().CompareTo(ia1.GetScore()));
        }

        public void DrawName(IA ia, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, SpriteFont font, Matrix view, Matrix projection)
        {
            // posicao 3D do texto 
            Vector3 textPosition = ia.Position + new Vector3(0, ia.GetScale() * 5, 0);

            // 3D para 2D
            Vector3 screenPosition = graphicsDevice.Viewport.Project(textPosition, projection, view, Matrix.Identity);

            // Verificar se a posição está na tela
            if (screenPosition.Z >= 0 && screenPosition.Z <= 1)
            {
                // Desenhar o texto
                string displayName = ia.name; // Nome da IA
                Vector2 textSize = font.MeasureString(displayName);
                Vector2 textScreenPosition = new Vector2(screenPosition.X - textSize.X / 2, screenPosition.Y - textSize.Y / 2);

                spriteBatch.DrawString(font, displayName, textScreenPosition, Color.White);
               
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice gd)
        {
            
            
            //MOSTRANDO RANKING
            for (var i = 0; i < ranking.Count; i++)
            {
                if (ranking[i] != null)
                {
                    spriteBatch.DrawString(debugFont, ranking[i], new Vector2(10, 20 * i), Color.White);
                }
            }

            //MOSTRANDO DEBUG
            for (var i = 0; i < debugTexts.Count; i++)
            {
                if (debugTexts[i] != null)
                {
                    spriteBatch.DrawString(debugFont, debugTexts[i], new Vector2(700, 20 * i), Color.White);
                }
            }

            //MOSTRANDO NOME DAS IAS
            for (var i = 0; i < GameManager.Instance.ias.Count; i++)
            {
                IA currentIA = GameManager.Instance.ias[i];
                DrawName(currentIA, spriteBatch, gd, debugFont, GameManager.Instance.camera.ViewMatrix, GameManager.Instance.camera.ProjectionMatrix);
            }

            

            
        }
    }
}
