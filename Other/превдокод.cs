=== ===


=== Tower ===
class WeaponTowerAI
    WeaponComponent weaponComponent
    
    void Init(healthCollection)
        weaponComponent.Init()
        
    void Update()
        stateMashine.UpdateFrame(weaponComponent)
        
class WeaponComponent
    Buff attackSpeedBuff
    
    void Update()
        if(Time.time > tineNextAttack)
            PlayAttackAnimation(ref attackPeriod ...)
            tineNextAttack = Time.time + attackPeriod * attackSpeedBuff.GetValue(Time.timme)
            
        
HomingRocketRangeWeaponComponent
    void Attack(HealthComponent enemyHealth, Damage damage)
        homingRocket = Instantiate(homingRocketPrefab, damage)
        fireBall.Init(target)
        
BobmRangeWeaponComponent
    void Attack(HealthComponent enemyHealth, Damage damage)
        fireBall = Instantiate(fireBallPrefab, damage)
        fireBall.Init(target)        
    
=== Weapon ===

Bobm
	void Init(point, GetPointDamageFabric)
		bonbMove.Init(point1, point2)
		
	void Update()
		Move()
		
		if(move.IsNear)
            ApplyMassDamage()


HomingRocket
	void Init(target, GetHealthDamageFabric)
		
	void Update()
		if(!target)
            Stop()
        
        Move(target.position)
        if(IsNear)
            damageDealer = GetDamage()
            damageDealer.Apply(target)

===  ===

class DamageComponent
    HealthComponent health
    Buff slowingBuff
    Buff poisonBuff
    
    void Init()
        health.Death += OnDeath
        
    void SetDamage(damage)
        if damage.type == basic
            health.SetDamage(damage.value)
        
        if damage.type == cold
            GetSlowingComponent().Set(damage.value, damage.duration)
           
            
    void OnDeath()        
        slowingBuff?.Stop()
        poisonBuff?.Stop()

class DamageBuffComponent
    float value
    float endTime

    void Set(duration, value)
        ReplaceBuff(duration, damage)
        if(!isActive)
            isActive = true
            vfx.Play()    
    
    void Update()
            if(Time.time > endTime)
                isActive = false
                poisonVfx.Stop()
            else if
                health.SetDamage(poisonBuff.value * Time.deltaTime)

class SlowingBuffComponent
    float value
    float endTime

    void Set(duration, value)
        ReplaceBuff(duration, damage)
        moveComponent.slowingFactor = 1 - slowingBuff.GetValue()
        if(!isActive)
            isActive = true
            slowingVfx.Play()    
    
    void Update()
        if(Time.time > endTime)
            isActive = false
            slowingVfx.Stop()
            move.slowingFactor = 1    
    
    

class HealthComponent
    poisonBuff
    
    void Update()
        damageFromPoison = poisonBuff.GetValue(Time.time)
        damageFromPoison = Min(hp, damageFromPoison)
        hp-=damageFromPoison
        poisonBuff.statistics.AddPoisonDamage(damageFromPoison)
        
    void SetDamage(damage)
        if damage.type == basic
            hp-=damage.value
    
        if damage.type == cold
            coldBaff.Add(damage.duration, damage.value)
            
            
            
WayMoveComponent
    float slowingFactor
    
    void Update()
        Move(.... * GetSpeed())
    
    float GetSpeed()
        speed * slowingFactor
    

Buff
    float value
    float timeEnd
    DamageStatistics statistics
    
    flaot GetValue(time)
        coldBuff.timeEnd < Time.Time
    
===  ===    
    
    
    
    
    