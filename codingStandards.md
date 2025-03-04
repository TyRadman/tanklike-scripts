# Tanks Game – Development Rules
 
## Assets Naming
Every asset in the project should be named in the following format:
`<asset type>_<name>_<details if necessary>_<version>`
#### Example
`PF_Player_01`
`T_HealthBar_Fill_01`

The prefixes for each asset type are listed in the following table:

| Prefix  | Asset Type                         | Example            |
|---------|-----------------------------------|--------------------|
| PF      | Prefab                            | PF_Player         |
| M       | Material                          | M_Player          |
| T       | Texture                           | T_Player          |
| RT      | Raw Texture                       | RT_Minimap        |
| SK      | Skinned Mesh (models with armatures) | SK_Player      |
| SM      | Static Mesh (models with no armatures) | SM_Stone      |
| AC      | Animator                          | AC_Player         |
| Anim    | Animation                         | Anim_Player_Idle  |
| S       | Scene                             | S_MainScene       |
| VFX     | Visual Effects                    | VFX_Explosion     |
| VFXG    | Visual Effect Graph               | VFXG_Shield       |
| SG      | Shader Graphs                     | SG_Shield         |


 
### Special rules:
1. For scriptable objects, we make the prefix using the first letter of each word of the script’s name.
E.g. `AttackData -> AD_Sword`
2. For animations, we’ll have the format: `Anim_<object’s name>_<animation name>`
E.g. `Anim_Player_Run`
 

## Directories

Code
- All of our scripts will be placed inside the `Code` directory.
- The subdirectory Global will contain all the global scripts (which are inside our root namespace).
- We create a subdirectory for every sub namespace and put the corresponding scripts there.
 
Characters
- The characters folder will contain all the assets for the characters.
- For every character we create a folder with the name of that character (e.g. Player), and inside we create subfolders for the character assets:
```graphql
|--|Player
|  |--|Models
|  |  |--|Materials­
|  |  |--|Textures
|  |--|Weapons
|  |--|Abilities
|  |--|Etc
```

## Code Naming Convention
 
### Variables
#### Private and Protected Variables
Use Camel Case with an underscore prefix:
``` C#
private int _speed;
protected int _speed;

// same goes for serialized fields
[SerializeField] private int _speed;
[SerializeField] protected int _protectedSpeed;
```
#### Public Fields and Properties
Use PascalCase:
``` C#
public PlayersManager PlayersManager;
[field: SerializeField] public GameManager GlobalGameManager { set; private set; } 

// same goes for hidden fields
[HideInInspector] public bool IsActive;
public Transform Transform { get; private set; }
```

#### Constant Variables 
Use uppercase letters and underscores to separate words:
``` C#
public float PLAYER_TANK_SIZE = 1.5f;
```
 
#### Functions
Functions use PascalCase, and function parameters use Camel Case:
``` C#
public void TakeDamage(int damage)
{
  // logic
}
```

#### Enums
Use PascalCase for the enum’s name prefixed by “E”:
``` C#
public enum ItemType
{
  Sword = 0,
  LongSword = 1,
}
```
Note: make sure you always number the enum values, so that Unity doesn't lose manually assigned values in the inspector when modifying the enum.

Classes and Interfaces
Classes and interfaces use PascalCase with the Interfaces prefixed with "I":
``` C#
// Class
public class Player
{
 
}

// Interface
public interface IDamageable
{
     	
}
```

Events
For events, we will mostly `System.Action` and `System.Func` as our default delegate. If more control over the events is need, then create your own custom delegate. Use PascalCase prefixed with "On":
``` C#
public System.Action OnHit;

public System.Func<bool> OnJump;
```

## Code Annotations & Task Tracking
To make sure that we’re keeping track of code that will need our attention later, we will use task tracking comments. Here’s a breakdown of all possible comments we have:

| Comment title | Description |
| - | - |
| `// TODO: ` | Things to complete. | 
| `// REVIEW: ` | Areas needing discussion. |
| `// REFACTOR: ` | Code that needs optimization. |


