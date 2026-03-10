***
the architecture here was a bit of a mess, and its relatively new

cb_solarsystem (mono) is the main script for the system, cb_solarsystemdata is everything that needs to be written to disk

cb_trackedbody (mono) is the main script for planets, cb_trackedbodydata is the disk-writing one

cb_trackedbodydata has some general data, and all the data from cbp_config and cbt_config

cbp_config also has a cbp_orbit class
***

cb --> celestial body

all the solar system rendering/calculation stuff, basically

subtypes are:
cbr --> cb rendering (shaders, perspective tricks)
cbp --> cb positioning (kepler/orbital stuff)
cbt --> cb terrain (terrain profiles and such)