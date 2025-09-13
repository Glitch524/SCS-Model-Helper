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

namespace Def_Writer.Windows
{
    /// <summary>
    /// PhysicsToyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PhysicsToyWindow : Page
    {
        public PhysicsToyWindow()
        {
            InitializeComponent();
        }
    }
}

//SiiNunit
//{
//accessory_addon_int_data : scaniatoy1.scania.rs.toyhang
//{
//name: "Scania toy 1 by golcan"
//	icon: "cube_white.dlc_toys"


//	price: 120
//	unlock: 0
//	interior_model: "/vehicle/truck/upgrade/interior_decors/toyhang/fastening.dlc_toys.pmd"
//	data[]: .scaniatoy1.phys_data
//}
//physics_toy_data: .scaniatoy1.phys_data
//{
//phys_model: "/vehicle/truck/upgrade/interior_decors/toyhang/scania_toy_1.pmd"
//	phys_model_coll: "/vehicle/truck/upgrade/interior_decors/toyhang/scania_toy_1.pmc"

//	toy_type: "TT_rope"			# TT_rope, TT_double_rope, TT_joint TT_joint_free
//	toy_mass: 0.75				# toy mass
//	linear_damping: 0.3				# damping of swinging of toy
//	linear_stiffness: 1.60				# rope - influences rope springing

//	locator_hook_offset: (0.0f, -0.008f, 0.0f)		# offset of connection point on hook against toy locator

//	# definitions valid only for toys with rope 所以选择rope后才会出现这些选项
//	rope_width: 0.002			# width of rope
//	rope_length: 0.11				# lengt of rope, in the case of double_rope, distance between hook and toy
//	rope_hook_offset: 0.0				# double_rope - distance between hooks, locator is in the middle
//	rope_toy_offset: 0.0				# double_rope - distance between rope tingles at toy
//	rope_resolution: 5				# number of inner nodes of rope, except end nodes with anchores
//	rope_linear_density: 0.5435				# density, i.e. weight of rope per 1 m of length
//	position_iterations: 5				# number of iterations in position solver
//	node_damping: 0.3				# rope node velocity damping

//  rope_material: "/material/ropes/rope_black.dlc_toys.mat"
//}
//}

//phys_model  string	default: ""
//phys_model_look token	default: default
//phys_model_variant token	default: default
//phys_model_coll string	default: ""
//toy_type    enum    default: TT_rope
//options: TT_double_rope, TT_joint, TT_joint_free, TT_rope
//toy_mass	float	default: 0.100000
//toy_cog_offset float3	default: (0.000000, 0.000000, 0.000000)
//linear_stiffness    float	default: 1.000000
//angular_stiffness float2	default: (0.200000, 0.200000)
//linear_damping  float	default: 0.500000
//angular_damping float2	default: (0.020000, 0.020000)
//angular_amplitude float3	default: (0.000000, 0.000000, 0.000000)
//node_damping    float	default: 0.000000
//locator_hook_offset float3	default: (0.000000, 0.000000, 0.000000)
//rest_position_offset float3	default: (0.000000, 0.000000, 0.000000)
//rest_rotation_offset float3	default: (0.000000, 0.000000, 0.000000)
//instance_offset array<float3>   default: []
//rope_width float   default: 0.500000
//rope_length float	default: 0.000000
//rope_hook_offset    float	default: 0.000000
//rope_toy_offset float	default: 0.000000
//rope_resolution uint	default: 8
//position_iterations uint	default: 10
//rope_linear_density float	default: 1.000000
//rope_material   string	default: "
