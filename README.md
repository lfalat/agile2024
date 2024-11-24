WARNING
==============

Aplikácia vyžaduje mať prítomnosť **appsettings.json** súboru, ktorý však kvôli bezpečnostným dôvodom nakoľko obsahuje connection stringy sme nedali na github, je potrebne si ich vypytať od študentov ktorí pracovali na tomto projekte.

Lokálne spustenie
==============

Požiadávky
-----------

Pre spustenie je potrebné mať stiahnuté Visual Studio 2022 spolu s .net 8 SDK.  

Spustenie
------------
Po otvorení ***.sln*** súboru je potrebné na bočnej lište dať pravé tlačidlo myši na ***Solution*** a zvoliť možnosť ***Restore NuGet packages***.  
Následne je možné spustiť aplikáciu. Pri lokálnom spustení aplikácia beží na adrese https://localhost:5092 

Debugging
------------
Po spustení aplikácie v debug režime sa otvorí stránka s názvom Swagger. Je to stránka na ktorej je možné otestovať endpointy tejto backend aplikácie.  
Z vlastných skúsenosti však odporúčam používať aplikáciu s názvom Postman, kde je testovanie endpointov lepšie.  


Nasadenie do produkcie
==================

Tento guide je miereny na nasadenie na Microsoft Azure.

Požiadávky
------------

Pre nasadenie je potrebné mať vytvorenú App Service službu s runtime stack .NET 8, MySQL databázu a Azure Blob Storage službu.  
Connection stringy na MySQL databázu a Azure Blob Storage službu je potrebné upraviť v **appsettings.json**

Proces nasadenia
-------------

Tento proces predpokladá s už vytvorenou App Service na Microsoft Azure.  
Po otvorení ***.sln*** súboru je potrebné na bočnej lište dať pravé tlačidlo myši na ***AGILE2024_BE*** a zvoliť možnosť ***Publish***.  
Následne je potrebne vytvoriť si Publishing profile. Target je **Azure**. Specific target je **Azure App Service (Windows)**.  
Ďalej sa otvorí okno kde môžete zvoliť už vytvorenú App Service službu.  
![image](https://github.com/user-attachments/assets/da68a60e-780b-41a7-a20d-b2d57183352b)  
Ako deployment type zvolime **Publish (generates pubxml file)**.  
Po vytvorení Publish profilu môžeme stlačiť tlačidlo **Publish** ktoré nasadí aplikáciu na Azure App Service službu.
