import React, { useEffect } from 'react';
import { useState } from 'react';
import Fetcher from '../ThreeBrowser/Fetcher';
import { Node } from './Node';
import { MdExpandMore, MdExpandLess } from 'react-icons/md';

const BrowserNode = (props: {parent: Node}) => {
    const [childNodes, setChildren] = useState<Node[]>([]);
    const [isExpanded, expand] = useState(false);

    async function onButtonClick() {
        expand(!isExpanded);
        const response = await fetchChildNodes(props.parent.Id);
        setChildren(response);
    }

    //add onclick to button, add to filters
    return (
        <ul>
            <li>
                <button>
                    {props.parent.Name}
                </button>
                {!isExpanded ? <MdExpandMore onClick={() => onButtonClick()}/> : 
                    <MdExpandLess onClick={() => onButtonClick()}/>}
            </li>
            {isExpanded ? <ul>
                {childNodes.length > 0 ? childNodes.map((node: Node) => 
                    <BrowserNode parent={node}/>)
                : <li>No further children</li>}
            </ul> : null }
        </ul>
    )
}

export const HierarchyBrowser = (props: {startNode: Node}) => {
    const [parentNode, setParent] = useState<Node|null>(null);
    const [childNodes, setChildren] = useState<Node[]>([]);

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

    return (
        <div>
            <ul className="parent node">
                {parentNode !== null ? <li><button>{parentNode.Name}</button></li> : <li>No further parent</li>}
                <ul className="start node">
                    <li><button>{props.startNode.Name}</button></li>
                    {childNodes.length > 0 ? 
                    <ul className="child node">
                        {childNodes.map(n => <BrowserNode parent={n}/>)}
                    </ul> : <li>No further children</li>}
                </ul>
            </ul>
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