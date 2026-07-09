=== ===
InstantiateBombTower(cell)
    // Надо внедрить:
    // 
    towerGo = Inst(prefab)
    towerGo.Get<WeaponTowerAI>().Init()

=== Tower ===
WeaponTowerAI
    WeaponComponent weaponComponent
    
    Init(healthCollection)
        weaponComponent.Init()
        
    Update()
        stateMashine.UpdateFrame(weaponComponent)
        
        
HomingRocketRangeWeaponComponent
    Attack(HealthComponent enemyHealth, Damage damage)
        homingRocket = Instantiate(homingRocketPrefab)
        fireBall.Init(target, GetHealthDamageFabric)
        
BobmRangeWeaponComponent
    Attack(HealthComponent enemyHealth, Damage damage)
        fireBall = Instantiate(fireBallPrefab)
        fireBall.Init(target, GetHealthDamageFabric)        
    
=== ===

Bobm
	Init(point, GetPointDamageFabric)
		bonbMove.Init(point1, point2)
		
	Update()
		Move()
		
		if(move.IsNear)
            ApplyMassDamage()


HomingRocket
	Init(target, GetHealthDamageFabric)
		
	Update()
		if(!target)
            Stop()
        
        Move(target.position)
        if(IsNear)
            damageDealer = GetDamage()
            damageDealer.Apply(target)