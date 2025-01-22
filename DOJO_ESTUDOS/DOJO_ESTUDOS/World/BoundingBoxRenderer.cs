using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DOJO_ESTUDOS
{
    public static class BoundingBoxRenderer
    {
        private static VertexPositionColor[] vertices;
        private static short[] indices;
        private static BasicEffect effect;

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            // Criação do BasicEffect
            effect = new BasicEffect(graphicsDevice)
            {
                VertexColorEnabled = true,
                LightingEnabled = false
            };
        }

        public static void Draw(GraphicsDevice graphicsDevice, BoundingBox boundingBox, Matrix view, Matrix projection, Color color)
        {
            if (effect == null) return;

            // Atualiza a matriz de View e Projection
            effect.View = view;
            effect.Projection = projection;

            // Criação dos vértices e índices (linhas do BoundingBox)
            CreateBoundingBoxVertices(boundingBox, color);

            // Ativa o efeito
            effect.CurrentTechnique.Passes[0].Apply();

            // Renderiza as linhas
            graphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.LineList,
                vertices,
                0,
                vertices.Length,
                indices,
                0,
                indices.Length / 2
            );
        }

        private static void CreateBoundingBoxVertices(BoundingBox box, Color color)
        {
            Vector3[] corners = box.GetCorners();

            // Cria os vértices
            vertices = new VertexPositionColor[8];
            for (int i = 0; i < 8; i++)
            {
                vertices[i] = new VertexPositionColor(corners[i], color);
            }

            // Índices para desenhar as 12 arestas do BoundingBox
            indices = new short[]
            {
                0, 1, 1, 2, 2, 3, 3, 0, // Base inferior
                4, 5, 5, 6, 6, 7, 7, 4, // Base superior
                0, 4, 1, 5, 2, 6, 3, 7  // Conexões entre as bases
            };
        }
    }
}
