# RelationInspectorDatabaseExample
Using Relation Inspectors to display links between Datas (Levels, Reward, Prefabs, etc) of a game database


This is based on [RelationsInspector](https://github.com/seldomU) by @seldomU.
This a very rough proof of concept for a use case where each levels gives one reward which is one prefab. Everything has to be unique and connected, and we want a way to visualize that.

- Shows how every data of a game is connected:

- The Database Hold references to levels
- Level Hold references to Rewards
- Rewards hold references to Prefabs
- Cell will display in red if some links don't exists, or don't match:  ex: Two levels have a common reward.


How to use:

1-  Windows > RelationsInspector
2- ![image](https://github.com/AlexStrook/RelationInspectorDatabaseExample/assets/12585149/aa4e5936-24ac-4e77-8ee7-94271f6870c6)
3- drag and drop "Master Database" in that windows. It will show the links between all elements. 

![image](https://github.com/AlexStrook/RelationInspectorDatabaseExample/assets/12585149/b9127d32-9ad0-4b3f-8e6d-eb694ad201a6)

The cells gets a bit wrangled, you can untangled them a bit and press "Relayout".

Check the RelationsInspector [page](https://github.com/seldomU/relationsinspector) for more details!

