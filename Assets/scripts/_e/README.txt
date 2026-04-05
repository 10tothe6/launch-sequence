the entity system is a lot more formal now, and there have been some nomenclature changes, so I may as well explain it all in one place.

there are now THREE types of entities, not two:

* fixed entities - always have a scale of 1, then un-render
* mimic entities - copy the scale of a celestial body
* floating entities - scales to maintain a sense of perspective
(there is a distinction to be made between celestial bodies and other floating entities)

not every entity has orbital data (a pConfig), only some. For planets, the e_floatingentity class is part of the cb_trackedbodydata class. The position and velocity fields take over the job of the cbp_poseinfo from before. since planets use a LOCAL position, I'm changing the floating entitiy code to allow for a parent.

MOST IMPORTANTLY:
the entity classes now have MONO COMPONENTS, following the convention set by cb_solarsystem + cb_solarsystemdata, as well as cb_trackedbody + cb_trackedbodydata. 

This arrangement makes it as easy as possible to save/load things and communicate between scripts