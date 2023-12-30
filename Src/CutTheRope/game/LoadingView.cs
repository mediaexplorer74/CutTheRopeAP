using GameManager.iframework;
using GameManager.iframework.core;
using GameManager.iframework.visual;
using GameManager.windows;
using Microsoft.Xna.Framework;

namespace GameManager.game
{
    internal class LoadingView : View
    {
        public bool game;

        private static Color s_Color1 = new Color(0.85f, 0.85f, 0.85f, 1f);

        public override void draw()
        {
            Global.MouseCursor.Enable(false);
            OpenGL.glEnable(0);
            OpenGL.glEnable(1);
            OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
            preDraw();
            CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
            int num = 126 + cTRRootController.getPack();
            float num2 = Application.sharedResourceMgr().getPercentLoaded();
            Texture2D texture = Application.getTexture(num);
            OpenGL.glColor4f(s_Color1);
            Vector quadSize = Image.getQuadSize(num, 0);
            float num3 = SCREEN_WIDTH / 2f - quadSize.x;
            GLDrawer.drawImageQuad(texture, 0, num3, 0.0);
            OpenGL.glPushMatrix();
            float num4 = SCREEN_WIDTH / 2f + quadSize.x / 2f;
            OpenGL.glTranslatef(num4, SCREEN_HEIGHT / 2f, 0.0);
            OpenGL.glRotatef(180.0, 0.0, 0.0, 1.0);
            OpenGL.glTranslatef(0f - num4, (0f - SCREEN_HEIGHT) / 2f, 0.0);
            GLDrawer.drawImageQuad(texture, 0, SCREEN_WIDTH / 2f, 0.5);
            OpenGL.glPopMatrix();
            Texture2D texture2 = Application.getTexture(5);
            if (!game)
            {
                OpenGL.glEnable(4);
                OpenGL.setScissorRectangle(0.0, 0.0, SCREEN_WIDTH, (double)(1200f * num2) / 100.0);
            }
            OpenGL.glColor4f(Color.White);
            num3 = Image.getQuadOffset(5, 0).x;
            GLDrawer.drawImageQuad(texture2, 0, num3, 80.0);
            num3 = Image.getQuadOffset(5, 1).x;
            GLDrawer.drawImageQuad(texture2, 1, num3, 80.0);
            if (!game)
            {
                OpenGL.glDisable(4);
            }
            if (game)
            {
                Vector quadOffset = Image.getQuadOffset(5, 3);
                float num5 = (float)(1250.0 * (double)num2 / 100.0);
                GLDrawer.drawImageQuad(texture2, 3, quadOffset.x, 700f - num5);
            }
            else
            {
                float num6 = (float)(1120.0 * (double)num2 / 100.0);
                GLDrawer.drawImageQuad(texture2, 2, 1084.0, (double)num6 - 100.0);
            }
            postDraw();
            OpenGL.glColor4f(Color.White);
            OpenGL.glDisable(0);
            OpenGL.glDisable(1);
        }
    }
}
