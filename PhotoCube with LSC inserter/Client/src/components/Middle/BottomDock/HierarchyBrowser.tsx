import React, { useEffect } from 'react';
import { useState } from 'react';
import Fetcher from '../ThreeBrowser/Fetcher';
import { Node } from './Node';
import { createFilter } from './TagsetFilter';
import { MdExpandMore, MdExpandLess } from 'react-icons/md';
import '../../../css/BottomDock/HierarchyBrowser.css'
import { Filter } from '../../Filter';

const BrowserNode = (props: {parent: Node}) => {
    const [childNodes, setChildren] = useState<Node[]>([]);
    const [isExpanded, expand] = useState(false);

    async function onButtonClick() {
        expand(!isExpanded);
        const response = await fetchChildNodes(props.parent.Id);
        setChildren(response);
    }

    return (
        <div className="hierarchy">
            <li className="hierarchy node">
                <button>
                    {props.parent.Name}
                </button>
                {!isExpanded ? <MdExpandMore className="expand hierarchy" onClick={() => onButtonClick()}/> : 
                    <MdExpandLess className="expand hierarchy" onClick={() => onButtonClick()}/>}
            </li>
            {isExpanded ? <ul className="hierarchy children">
                {(childNodes.length > 0) ? childNodes.map((node: Node) => 
                    <BrowserNode parent={node}/>)
                : <li><button disabled={true}>No further children</button></li>}
            </ul> : null }
        </div>
    )
}

export const HierarchyBrowser = (props: {startNode: Node}) => {
    const [parentNode, setParent] = useState<Node|null>(null);
    const [childNodes, setChildren] = useState<Node[]>([]);
    const [selectedNode, updateSelection] = useState<Node|null>(null);

    useEffect(() => {
        fetchParent(props.startNode.Id);
        fetchChildren(props.startNode.Id);
    }, [props.startNode])

    async function fetchParent(nodeId: number) {
        const response = await Fetcher.FetchParentNode(nodeId);
        if (response.length > 0) {
            const parent = response[0]
            setParent(parent);
        }
    }

    async function fetchChildren(nodeId: number) {
        const response = await fetchChildNodes(nodeId);
        setChildren(response);
    }

    const onButtonClick = (tagName: string, id: number) => {
        const filter: Filter = createFilter(tagName, id, "hierarchy");
    }

    //with onclick update selection, add button adds filter
    //disable button 
    return (
        <div className="hierarchy browser">
            <h5>Browse hierarchy:</h5>
            <ul className="scrollable hierarchy">
                {(parentNode !== null) ? <li className="hierarchy node"><button>{parentNode.Name}</button></li> : 
                    <li className="hierarchy node"><button disabled={true}>No further children</button></li>}
                <ul>
                    <li className="hierarchy node"><button>{props.startNode.Name}</button></li>
                    {(childNodes.length > 0) ? <ul className="hierarchy children">
                        {childNodes.map(n => <BrowserNode parent={n}/>)}</ul> :
                        <ul><li className="hierarchy node"><button disabled={true}>No further children</button></li></ul>}
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