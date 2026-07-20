namespace MetaUpgrade


class UpgradeTree
    Node[] allNodes
    
    void Start()
        for node in allNodes
            state = node.CalculateState(playerBank)
            node.State = state
            node.DarwState(state)
            
        for node in allNodes
            node.Draw(playerBank, immedate:true)
            
    void OnClickNode(node)
        if node.State == CanOpen
            node.upgrade.Open()
            playerBank -= node.upgrade.Cost
            
            for node in allNodes.Exclude(node.nextNodes).Exclude(node)
                state = node.CalculateState(playerBank)
                if satate != node.State
                    Asser(state == NotEnought)
                    node.DrawNotEnoughtWithAniamtion()
                    
            node.DrawOpenWithAnimation()
            if(node.nextNodes.Count == 0)
                return
            
            for line in node.Lines
                line.DrawAnimation()
            await node.Lines[0].AnimationLength
            
            for node in node.nextNodes
                newState = node.GetState()
                if(newState == CanOpen)
                   node.DrawCanOpenWithAnimation()
                if(newState == NotEnought)
                   node.DrawCanOpenWithAnimation()
        else
            ShowHint(node.GetHint())

enum NodeState
    NotSet = 0,
    PreviousNotOpened = 1,
    NotEnought = 2,
    CanOpen = 3,
    Opened = 4

class Node
    Node[] nextNodes
    LineVfx[] lines


    int TotalPreviousCounter
    int OpenedPreviousCounter
    
    bool IsPreviusOpened => TotalPreviousCounter == OpenedPreviousCounter

    event Click

    void InitTree()
        for node in nextNodes
            node.TotalPreviousCounter++
    
    NodeState CalculateState(playerBank)
        if upgrade.IsOpen
            return Opened
        if !IsPreviusOpened
            return PreviousNotOpened
        if upgrade.Cost > playerBank
            return NotEnought
        return CanOpen
                    
    void OnClickOpen
        Click.Invoke(this)
}