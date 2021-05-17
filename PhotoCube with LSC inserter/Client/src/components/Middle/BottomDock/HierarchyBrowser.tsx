import React, { useEffect } from 'react';
import { useState } from 'react';
import Fetcher from '../ThreeBrowser/Fetcher';
import { Node } from './Node';
import { createFilter } from './TagsetFilter';
import { Filter } from '../../Filter';
import { MdExpandMore, MdExpandLess } from 'react-icons/md';
import '../../../css/BottomDock/HierarchyBrowser.css'

const BrowserNode = 
    (props: {node: Node, fetchChildren: () => Promise<any>, 
        hideChildren: () => void, select: (node: Node) => void}) => {

    const [isExpanded, expand] = useState(false);

    const onExpand = () => {
        props.fetchChildren();
        expand(true);
    }

    const onCollapse = () => {
        expand(false);
        props.hideChildren();
    }

    return (
        <li className="hierarchy node">
            <label>
                <input onClick={() => props.select(props.node)} type="radio" name="node"/>
                {props.node.Name}
            </label><br/>
            {!isExpanded ? 
            <MdExpandMore className="expand hierarchy" onClick={() => onExpand()}/>  
            :  <MdExpandLess className="expand hierarchy" onClick={() => onCollapse()}/>}
        </li>
    )
}

const BrowserNodeWithChildren = 
    (props: {parent: Node, showChildren: boolean, select: (node: Node) => void}) => {
    const [childrenShown, showChildren] = useState(false);
    const [childNodes, setChildren] = useState<Node[]|null>(null);

    useEffect(() => {
        //hide list of previous children
        showChildren(false);
        //show immediate children of startnode
        if (props.showChildren) {
            getChildren();
        }
    }, [props.parent.Id])

    async function getChildren() {
        if (props.showChildren || childNodes === null) {
            await fetchChildNodes(props.parent.Id).then(response => {
                setChildren(response);
            });
        } 
        showChildren(true);
    }

    const hideChildren = () => {
        showChildren(false);
    }

    return (
        <div className="hierarchy">
            <BrowserNode node={props.parent} fetchChildren={getChildren} hideChildren={hideChildren} select={props.select}/>
            {childrenShown ? 
            <ul className="hierarchy children">
                {(childNodes !== null && childNodes!.length > 0) ? childNodes!.map((node: Node) => 
                    <BrowserNodeWithChildren parent={node} showChildren={false} select={props.select}/>)
                    : <li><button disabled={true}>No further children</button></li>}
            </ul> : null }
        </div>
    )
}

export const HierarchyBrowser = 
    (props: {startNode: Node, activeFilters: Filter[],
         onFiltersChanged: (filter: Filter) => void}) => {
    const [parentNode, setParent] = useState<Node|null>(null);
    const [selectedNode, updateSelection] = useState<Node|null>(null);

    useEffect(() => {
        fetchParent(props.startNode.Id);
    }, [props.startNode.Id])

    async function fetchParent(nodeId: number) {
        const response = await Fetcher.FetchParentNode(nodeId);
        if (response !== null) {
            setParent(response);
        } else {
            setParent(null);
        }
    }

    const onButtonClick = () => {
        const filter: Filter = createFilter(selectedNode!.Name, selectedNode!.Id, "hierarchy");
        if (!props.activeFilters.some(af => af.Id === filter.Id)) {
            props.onFiltersChanged(filter);
        }
    }

    return (
        <div className="hierarchy browser">
            <h5>Browse hierarchy:</h5>
            <ul className="scrollable hierarchy">
                {(parentNode !== null) ? <li id="parent" className="hierarchy node"><label><input type="radio" name="node"/>{parentNode.Name}</label></li> 
                : <li className="hierarchy node"><button disabled={true}>No further parent</button></li>}
                <ul>
                    <BrowserNodeWithChildren parent={props.startNode} showChildren={true} select={updateSelection}/> 
                </ul>
            </ul>
            <button className="add button hierarchy" disabled={selectedNode === null} onClick={() => onButtonClick()}>Add filter</button>
        </div>
    )
}

//utility function
async function fetchChildNodes(nodeId: number){
    const response = await Fetcher.FetchChildNodes(nodeId);
    let children = [];
        if (response.length > 0) {
            children = response.map((node: Node) => { return { Id: node.Id, Name: node.Name}});
        }
    return children;
}