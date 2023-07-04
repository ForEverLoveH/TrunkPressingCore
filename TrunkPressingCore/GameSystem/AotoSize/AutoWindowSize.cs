using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrunkPressingCore.GameSystem 
{
    public class AutoWindowSize
    {
         
        public struct ControlRect
        {
            public int Left;
            public int Top;
            public int Wight;
            public int Height;
        }
        private  static  List<ControlRect> ControlRects= new List<ControlRect>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        public void ControlInitializeSize(Form form)
        {
            if(ControlRects.Count>0) ControlRects.Clear();
            ControlRect  controlRect = new ControlRect();
            controlRect.Left = form.Left; 
            controlRect.Top = form.Top;
            controlRect.Wight = form.Width;
            controlRect.Height = form.Height;
            ControlRects.Add(controlRect);
            foreach(Control control in form.Controls)
            {
                ControlRect rect = new ControlRect();
                rect.Left = control.Left;
                rect.Top = control.Top;
                rect.Wight = control.Width;
                rect.Height = control.Height;
                ControlRects.Add(rect);

            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        public void     ControlAutoSize(Form form)
        {
            float wScal = (float)form.Width / (float)ControlRects[0].Wight;
            float hScal = (float)form.Height / (float)ControlRects[0].Height;
            int ctrLeft, ctrTop  ,ctrWidth,ctrHeight    ;
            int ctrNo = 1;
            foreach(Control control in form.Controls)
            {
                ctrLeft = ControlRects[ctrNo].Left;
                ctrTop = ControlRects[ctrNo].Top;
                ctrWidth = ControlRects[ctrNo].Wight;
                ctrHeight = ControlRects[ctrNo].Height;
                control.Left = (int) (ctrLeft*wScal);
                control.Top =(int) (ctrTop*hScal);
                control.Width=(int) (ctrWidth*wScal);
                control.Height =(int) (ctrHeight*hScal);
                ctrNo++;
            }
           
        }
    }
}
