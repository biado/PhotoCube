import React, { useEffect } from 'react';
import { useState } from 'react';
import Fetcher from '../ThreeBrowser/Fetcher';
import { Node } from './Node';
import { createFilter } from './TagsetFilter';
import { Filter } from '../../Filter';
import { MdExpandMore, MdExpandLess } from 'react-icons/md';
import '../../../css/BottomDock/HierarchyBrowser.css'

const BrowserNode = 
    (props: {node: Node, fetchChildren: () => Promise<any>, hideChildren: () => void}) => {

    const [isExpanded, expand] = useState(false);
    const [isSelected, updateSelection] = useState(false);

    const onButtonClick = () => {
        updateSelection(!isSelected);
    }

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
            {isSelected ?
            <button className="active" onClick={() => onButtonClick()}>
                {props.node.Name}
            </button> 
            : <button onClick={() => onButtonClick()}>
                {props.node.Name}
            </button> 
            }
            {!isExpanded ? 
            <MdExpandMore className="expand hierarchy" onClick={() => onExpand()}/>  
            :  <MdExpandLess className="expand hierarchy" onClick={() => onCollapse()}/>}
        </li>
    )
}

const BrowserNodeWithChildren = (props: {parent: Node, showChildren: boolean}) => {
    const [childrenShown, showChildren] = useState(false);
    const [childNodes, setChildren] = useState<Node[]|null>(null);

    useEffect(() => {
        if (props.showChildren) {
            fetchChildren().then(() => showChildren(true));
        }
    }, [])

    async function fetchChildren() {
        if (childNodes === null) {
            const response = await fetchChildNodes(props.parent.Id);
            setChildren(response);
        }
        showChildren(true);
    }

    const hideChildren = () => {
        showChildren(false);
    }

    return (
        <div className="hierarchy">
            <BrowserNode node={props.parent} fetchChildren={fetchChildren} hideChildren={hideChildren}/>
            {childrenShown ? 
            <ul className="hierarchy children">
                {(childNodes!.length > 0) ? childNodes!.map((node: Node) => 
                    <BrowserNodeWithChildren parent={node} showChildren={false}/>)
                    : <li><button disabled={true}>No further children</button></li>}
            </ul> : null }
        </div>
    )
}

export const HierarchyBrowser = (props: {startNode: Node}) => {
    const [parentNode, setParent] = useState<Node|null>(null);

    useEffect(() => {
        fetchParent(props.startNode.Id);
    }, [props.startNode])

    async function fetchParent(nodeId: number) {
        const response = await Fetcher.FetchParentNode(nodeId);
        if (response.length > 0) {
            const parent = response[0]
            setParent(parent);
        }
    }

    return (
        <div className="hierarchy browser">
            <h5>Browse hierarchy:</h5>
            <ul className="scrollable hierarchy">
                {(parentNode !== null) ? <li className="hierarchy node"><button>{parentNode.Name}</button></li> : 
                    <li className="hierarchy node"><button disabled={true}>No further parent</button></li>}
                <ul>
                    <BrowserNodeWithChildren parent={props.startNode} showChildren={true}/> 
                </ul>
            </ul>
            <button className="add button hierarchy">Add filter</button>
        </div>
    )
}

// utility function
async function fetchChildNodes(nodeId: number){
    const response = await Fetcher.FetchChildNodes(nodeId);
    let children = [];
        if (response.length > 0) {
            children = response.map((node: Node) => { return { Id: node.Id, Name: node.Name}});
        }
    return children;
}