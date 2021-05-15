import React, { useState } from 'react';
import '../../../css/BottomDock/HierarchyFilter.css';
import Fetcher from '../ThreeBrowser/Fetcher';
import { HierarchyBrowser } from './HierarchyBrowser';
import { Node } from './Node';
import { Option } from './Option';

const SearchResults = (props: {
    options: Option[], onOptionSelected: (e: React.MouseEvent<HTMLSelectElement, MouseEvent>) => void}) => {
    return(
        <div className="search results">
            <h5>{props.options.length} occurence(s) found:</h5>
            <select onClick={e => props.onOptionSelected(e)} id="node-dropdown">
                {props.options.map(o => <option value={JSON.stringify(o)}>{o.NodeName}:{o.ParentnodeName}</option>)}
            </select> 
        </div>
    )
}

export const HierarchyExplorer = () => {
    const [input, updateInput] = useState<string>("");
    const [options, updateOptions] = useState<Option[]>([]);
    const [selectedNode, updateSelection] = useState<Node|null>(null);

    const onInputGiven = (input: string) => {
        updateInput(input);
    }

    const onOptionSelected = (e: React.MouseEvent<HTMLSelectElement, MouseEvent>) => {
        e.preventDefault();
        const selected: Option = JSON.parse(e.currentTarget.value);
        const node: Node = {
            Id: selected.NodeId,
            Name: selected.NodeName,
            ParentNode: null
        }
        updateSelection(node);
    }

    async function onSearch(e: React.MouseEvent<HTMLButtonElement, MouseEvent>){
        e.preventDefault();
        //set input to lowercase! or maybe not :D
        //return "no results" if response empty
        const response = await Fetcher.FetchNodeByName(input);
        const options = response.map((node: Node) => ({
            NodeId: node.Id,
            NodeName: node.Name,
            ParentnodeName: node.ParentNode !== null ? node.ParentNode!.Name : null
        }) as Option);
        updateOptions(options);
    }

    return (
        <div className="Filter">
            <form method="get">
                <input className="search field" type="text" placeholder="Search hierarchies" 
                    onChange={e => onInputGiven(e.target.value)}/>
            </form>
            <button className="submit button" type="submit" onClick={e => onSearch(e)}>Search</button>
            {(options.length > 0) ? <SearchResults options={options} onOptionSelected={onOptionSelected}/> : null }
            {selectedNode !== null ? <HierarchyBrowser startNode={selectedNode}/> : null }
        </div>
    )
}