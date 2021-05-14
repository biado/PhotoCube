import React, { useState } from 'react';
import '../../../css/HierarchyFilter.css';
import Fetcher from '../ThreeBrowser/Fetcher';
import { HierarchyBrowser } from './HierarchyBrowser';
import { Node } from './Node';
import { Option } from './Option';

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
        <div className="hierarchy explorer">
            <form method="get">
                <div className="search-bar">
                    <input type="text" placeholder="Enter tag to search hierarchies" onChange={e => onInputGiven(e.target.value)}/>
                    <button type="submit" onClick={e => onSearch(e)}>Search</button>
                </div>
                <select onClick={e => onOptionSelected(e)} name="node dropdown">
                    {options.map(o => 
                        <option value={JSON.stringify(o)}>{o.NodeName}:{o.ParentnodeName}</option>
                    )}
                 </select>
            </form>
            {selectedNode !== null ? <HierarchyBrowser startNode={selectedNode}/> : null }
        </div>
    )
}