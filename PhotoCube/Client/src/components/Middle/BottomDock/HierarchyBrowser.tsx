import React, { useEffect } from 'react';
import { useState } from 'react';
import Fetcher from '../CubeBrowser/Fetcher';
import { Node } from './Node';
import { createFilter } from './TagsetFilter';
import { Filter } from '../../Filter';
import { MdExpandMore, MdExpandLess } from 'react-icons/md';
import '../../../css/BottomDock/HierarchyBrowser.css'

/**
 * Component for displaying a single node whose immediate children can be shown or hidden.
 */
const BrowserNode = 
    (props: {node: Node, fetchChildren: () => Promise<any>, 
        hideChildren: () => void, onSelect: (node: Node) => void}) => {

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
        <li key={props.node.id} className="hierarchy node">
            <label>
                <input onClick={() => props.onSelect(props.node)} type="radio" name="node"/>
                {props.node.name}
            </label><br/>
            {!isExpanded ? 
            <MdExpandMore className="expand hierarchy" onClick={() => onExpand()}/>  
            :  <MdExpandLess className="expand hierarchy" onClick={() => onCollapse()}/>}
        </li>
    )
}

/**
 * Component for fetching and displaying a node's immediate children. 
 */
const BrowserNodeWithChildren = 
    (props: {parent: Node, showChildren: boolean, onSelect: (node: Node) => void}) => {
    const [childrenShown, showChildren] = useState(false);
    const [childNodes, setChildren] = useState<Node[]|null>(null);

    useEffect(() => {
        //hide list of previous children
        showChildren(false);
        //show immediate children of startnode
        if (props.showChildren) {
            getChildren();
        }
    }, [props.parent.id])

    async function getChildren() {
        if (props.showChildren) {
            await fetchChildNodes(props.parent.id).then(response => {
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
            <BrowserNode node={props.parent} fetchChildren={getChildren} hideChildren={hideChildren} onSelect={props.onSelect}/>
            {childrenShown ? 
            <ul className="hierarchy children">
                {(childNodes !== null && childNodes!.length > 0) ? childNodes!.map((node: Node) => 
                //  <div>{node.name}</div>)
                    <BrowserNodeWithChildren parent={node} showChildren={false} onSelect={props.onSelect}/>)
                    : <li key={0}><button disabled={true}>No further children</button></li>}
            </ul> : null }
        </div>
    )
}

/**
 * Component for browsing a hierarchy.
 * A node is selected from the search results and its immediate parent and children are shown.
 */
export const HierarchyBrowser = 
    (props: {startNode: Node, activeFilters: Filter[],
         onFiltersChanged: (filter: Filter) => void}) => {
    const [parentNode, setParent] = useState<Node|null>(null);
    const [selectedNode, updateSelection] = useState<Node|null>(null);
    const [buttonDisabled, disableButton] = useState<boolean>(false);

    useEffect(() => {
        fetchParent(props.startNode.id);
    }, [props.startNode.id])

    async function fetchParent(nodeId: number) {
        const response = await Fetcher.FetchParentNode(nodeId);
        if (response !== null) {
            setParent(response);
        } else {
            setParent(null);
        }
    }

    const onSelect = (node: Node) => {
        // updateSelection(node);
        // disableButton(props.activeFilters.some(af => af.Id === node.id));
        const filter: Filter = createFilter(node.name, node.id, "hierarchy");
        if (!props.activeFilters.some(af => af.id === filter.id)) {
            props.onFiltersChanged(filter);
            disableButton(true);
        }
        updateSelection(node);
        disableButton(props.activeFilters.some(af => af.id === node.id));
    }

    const onButtonClick = () => {
        const filter: Filter = createFilter(selectedNode!.name, selectedNode!.id, "node");
        if (!props.activeFilters.some(af => af.id === filter.id)) {
            props.onFiltersChanged(filter);
            disableButton(true);
        }
    }

    return (
        <div className="hierarchy browser">
            <h5>Browse hierarchy:</h5>
            <ul className="scrollable hierarchy">
                {(parentNode !== null) ? 
                <li key={parentNode.id} id="parent" className="hierarchy node">
                    <label>
                        <input onChange={() => onSelect(parentNode)} type="radio" name="node"/>{parentNode.name}
                    </label>
                </li> 
                : <li key={0} className="hierarchy node"><button disabled={true}>No further parent</button></li>}
                <ul>
                    <BrowserNodeWithChildren parent={props.startNode} showChildren={true} onSelect={onSelect}/> 
                </ul>
            </ul>
            {/* <button className="add button hierarchy" disabled={buttonDisabled} onClick={() => onButtonClick()}>Add filter</button> */}
        </div>
    )
}

//utility function
async function fetchChildNodes(nodeId: number){
    const response = await Fetcher.FetchChildNodes(nodeId);
    let children = [];
        if (response.length > 0) {
            children = response.map((node: Node) => { return { id: node.id, name: node.name}});
        }
    return children;
}