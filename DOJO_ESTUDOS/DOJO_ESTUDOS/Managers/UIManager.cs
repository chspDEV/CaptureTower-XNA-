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

        private List<SpriteFont> fonts;
        
        float elapsedTime = 0;
        int frameCount = 0;
        float fps = 0;

        public UIManager(List<SpriteFont> fonts)
        {
            if (Instance != null)
            {
                throw new InvalidOperationException("DebugManager já foi instanciado.");
            }

            Instance = this;
            this.fonts = fonts;
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
                // Calcular o FPS
                fps = frameCount / elapsedTime;

                // Exibir o FPS formatado
                SetText(debugTexts, 0, "FPS: " + fps.ToString("0.00"));

                // Exibir a distância de renderização
                SetText(debugTexts, 2, "Render Distance: " + GameManager.Instance.camera.renderDistance.ToString());

                // Resetar os valores para o próximo segundo
                frameCount = 0;
                elapsedTime = 0.0f;  // Garantir que o tempo acumulado seja resetado de forma limpa
            }
        }

        public void Update(GameTime gt)
        {
            FPS(gt);

            for (var i = 0; i < GameManager.Instance.ias.Count; i++)
            {
                IA currentIA = GameManager.Instance.ias[i];

                string txt;

                //TOP Mostrando com detalhes apenas o top 10
                if (i < 10) 
                {
                    txt = (i + 1).ToString() + " - HP(" + currentIA.GetHealth() + "/100) " + currentIA.name + " " + currentIA.GetScore() + "pts";
                
                }
                else 
                {
                    txt = (i + 1).ToString() + " - " + currentIA.name + " " + currentIA.GetScore() + "pts";
                }
                 
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

        public void DrawName(IA instance, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, SpriteFont font, Matrix view, Matrix projection)
        {
            // posicao 3D do texto 
            Vector3 textPosition = instance.Position + new Vector3(0, instance.GetScale() * 5, 0);

            // 3D para 2D
            Vector3 screenPosition = graphicsDevice.Viewport.Project(textPosition, projection, view, Matrix.Identity);

            // Verificar se a posição está na tela
            if (screenPosition.Z >= 0 && screenPosition.Z <= 1)
            {
                // Desenhar o texto
                string displayName = instance.name; // Nome da IA
                Vector2 textSize = font.MeasureString(displayName);
                Vector2 textScreenPosition = new Vector2(screenPosition.X - textSize.X / 2, screenPosition.Y - textSize.Y / 2);

                Vector2 debugOffset = new Vector2(0, 15f);

                spriteBatch.DrawString(font, displayName, textScreenPosition, Color.White);

                if (GameManager.Instance.activeDebug)
                {
                    spriteBatch.DrawString(font, instance.GetState().ToString(), textScreenPosition - debugOffset, Color.White);
                    spriteBatch.DrawString(font, instance.GetHealth().ToString(), textScreenPosition + debugOffset, Color.White);

                    string str = "";

                    for (var i = 0; i < instance.GetProjectiles().Count; i++) 
                    {
                        str += instance.GetProjectiles()[i].identifier;
                    } 

                    spriteBatch.DrawString(font, str, textScreenPosition + debugOffset * 2f, Color.Red);
                }
                    
               
            }
        }

        public void DrawName(Tower instance, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, SpriteFont font, Matrix view, Matrix projection)
        {
            // posicao 3D do texto 
            Vector3 textPosition = instance.Position + new Vector3(0, instance.GetScale() * 5, 0);

            // 3D para 2D
            Vector3 screenPosition = graphicsDevice.Viewport.Project(textPosition, projection, view, Matrix.Identity);

            // Verificar se a posição está na tela
            if (screenPosition.Z >= 0 && screenPosition.Z <= 1)
            {
                string displayName ="";
 
                // Desenhar o texto
                if (instance.owner != null)
                {
                    displayName = "[" + instance.owner.name + "]"; // Nome da IA
                }
                else
                {
                    displayName = "";
                }
                
                
                Vector2 textSize = font.MeasureString(displayName);
                Vector2 textScreenPosition = new Vector2(screenPosition.X - textSize.X / 2, screenPosition.Y - textSize.Y / 2);

                spriteBatch.DrawString(font, displayName, textScreenPosition, Color.White);

            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice gd)
        {

            //MOSTRANDO DEBUG
            for (var i = 0; i < debugTexts.Count; i++)
            {
                if (debugTexts[i] != null)
                {
                    spriteBatch.DrawString(fonts[0], debugTexts[i], new Vector2(1100, 20 * i), Color.White);
                }
            }
            
            //MOSTRANDO RANKING
            for (var i = 0; i < ranking.Count; i++)
            {
                if (ranking[i] != null)
                {
                    SpriteFont fnt;

                    if (i > 9) // ESTA FORA DO PODIO
                    {
                        fnt = fonts[4];
                        spriteBatch.DrawString(fnt, ranking[i], new Vector2(10, (20 * 9 - 50) + i * 7.5f), Color.White);
                    }
                    else // esta dentro do podio
                    {
                        fnt = fonts[1];
                        spriteBatch.DrawString(fnt, ranking[i], new Vector2(10, 20 * i + 10), Color.White);
                    } 

                    
                }
            }

            //MOSTRANDO NOME DAS IAS
            for (var i = 0; i < GameManager.Instance.ias.Count; i++)
            {
                IA currentIA = GameManager.Instance.ias[i];
                DrawName(currentIA, spriteBatch, gd, fonts[2], GameManager.Instance.camera.ViewMatrix, GameManager.Instance.camera.ProjectionMatrix);
            }

            //MOSTRANDO O NOME DE QUEM CAPTUROU AS TORRES
            for (var i = 0; i < GameManager.Instance.towers.Count; i++)
            {
                Tower currentTower = GameManager.Instance.towers[i];
                DrawName(currentTower, spriteBatch, gd, fonts[2], GameManager.Instance.camera.ViewMatrix, GameManager.Instance.camera.ProjectionMatrix);
            }

            //MOSTRANDO TIMER
            spriteBatch.DrawString(fonts[3], GameManager.Instance.GetTimerText(), new Vector2(560, 5), Color.White);
        }
    }
}
