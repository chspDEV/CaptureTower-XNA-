using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DOJO_ESTUDOS
{
    public class Ground
    {
        private VertexPositionColor[] vertices;  // Vértices do chão
        private short[] indices;  // Índices para formar os triângulos
        private BasicEffect effect;  // Efeito básico para renderizar o chão
        private Color color;

        public float scale;  // Escala do chão

        public Ground(GraphicsDevice graphicsDevice, Color color, float scale = 1.0f)
        {
            this.scale = scale;
            this.color = color;

            // Inicializando os vértices do chão (um plano simples)
            vertices = new VertexPositionColor[4]
            {
                new VertexPositionColor(new Vector3(-1, 0, -1) * scale, Color.Green),  // Vértice inferior esquerdo
                new VertexPositionColor(new Vector3(1, 0, -1) * scale, Color.Green),   // Vértice inferior direito
                new VertexPositionColor(new Vector3(-1, 0, 1) * scale, Color.Green),   // Vértice superior esquerdo
                new VertexPositionColor(new Vector3(1, 0, 1) * scale, Color.Green)     // Vértice superior direito
            };

            // Definindo os índices para os dois triângulos que formam o plano
            indices = new short[]
            {
                0, 1, 2,  // Primeiro triângulo
                2, 1, 3   // Segundo triângulo
            };

            // Inicializando o efeito básico para desenhar
            effect = new BasicEffect(graphicsDevice)
            {
                VertexColorEnabled = true  // Ativar cores dos vértices
            };
        }

        public void Draw(GraphicsDevice graphicsDevice, Matrix view, Matrix projection)
        {
            // Configurar o efeito para desenhar os vértices
            effect.World = Matrix.Identity;   // O chão não precisa de transformação mundial
            effect.View = view;               // Matriz de visão da câmera
            effect.Projection = projection;   // Matriz de projeção da câmera

            // Aplicar o efeito e desenhar o chão
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                // Desenhar o chão (triângulos)
                graphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleList,  // Tipo de primitiva (triângulo)
                    vertices,  // Vértices a serem usados
                    0,  // Offset dos vértices
                    vertices.Length,  // Número de vértices
                    indices,  // Índices para formar os triângulos
                    0,  // Offset dos índices
                    indices.Length / 3  // Número de triângulos
                );
            }
        }
    }
}
