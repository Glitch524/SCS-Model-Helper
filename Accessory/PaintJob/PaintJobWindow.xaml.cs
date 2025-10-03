using SCS_Mod_Helper.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SCS_Mod_Helper.Accessory.PaintJob
{
    /// <summary>
    /// PaintJobWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PaintJobWindow : BaseWindow
    {
        public PaintJobWindow()
        {
            InitializeComponent();
        }
    }
}


//Let's try to encode via sRGB Transformations manually...

//We have color 235.
//Let f denote a fractional (from the word "float") number.

//We encode "directly" (according to the upper formula).
//f = 235 / 255 = 0.92156862745098039215686274509804.
//The number f turned out to be greater than 0.04045, so we consider this: ((f + 0.055) / 1.055) and then raise it to the power of 2.4.
//We get: 0.83076987677465456326680486629645.That is as in def.

//We encode "inversely" (according to the lower formula).
//Number f = 0.8307 (taken from def).
//We have a number f greater than 0.0031308, so we consider this: we raise f to the power (1/2.4), then multiply by 1.055, then subtract 0.055.
//We get: 0.92153440159716157976920448100598.Multiplying it by 255, we get 234.99127240727620284114714265653, i.e., rounded up, 235.
