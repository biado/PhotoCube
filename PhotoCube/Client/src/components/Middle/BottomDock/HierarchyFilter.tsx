import React, { useEffect, useState } from 'react';
import '../../../css/BottomDock/HierarchyFilter.css';
import { Filter } from '../../Filter';
import Fetcher from '../CubeBrowser/Fetcher';
import { HierarchyBrowser } from './HierarchyBrowser';
import { Node } from './Node';
import { Option } from './Option';


/**
 * Component for displaying search results.
 * Hierarchy nodes associated with the same tag will be displayed as separate options. 
 */
const SearchResults = (props: {
    options: Option[], onOptionSelected: (e: React.ChangeEvent<HTMLSelectElement>) => void}) => {
    const [selected, updateSelection] = useState<string>("");

     //reset dropdown when options are updated
    useEffect(() => {
        if (selected.length !== 0) { updateSelection(""); }
    }, [props.options])

    const onOptionSelected = (e: React.ChangeEvent<HTMLSelectElement>) => {
        props.onOptionSelected(e);
        updateSelection(e.currentTarget.value);
    }

    return(
        <div className="search results">
            <h5>{props.options.length} occurence(s) found:</h5>
            <select value={selected} onChange={e => onOptionSelected(e)} className="result dropdown">
                <option key={0} value="">Select filter</option>
                {props.options.map(o => <option key={o.NodeId} value={JSON.stringify(o)}>{o.NodeName}:{o.ParentnodeName}</option>)}
            </select> 
        </div>
    )
}

/**
 * Component for browsing hierarchies and adding filters.
 * Consists of a search field, a search results component and the hierarchy browser.
 */
export const HierarchyExplorer = (props: {onFiltersChanged: (filter: Filter) => void, activeFilters: Filter[]}) => {
    const [input, updateInput] = useState<string>("");
    const [options, updateOptions] = useState<Option[]>([]);
    const [selectedNode, updateSelection] = useState<Node|null>(null);

    const onInputGiven = (input: string) => {
        updateInput(input);
    }

    const onOptionSelected = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const selected: Option = JSON.parse(e.currentTarget.value);
        const node: Node = {
            id: selected.NodeId,
            name: selected.NodeName,
            parentNode: null
        }
        updateSelection(node);
    }

    async function onSearch(e: React.MouseEvent<HTMLButtonElement, MouseEvent>){
        e.preventDefault();
        const response = await Fetcher.FetchNodeByName(input);
        const options = response.map((node: Node) => ({
            NodeId: node.id,
            NodeName: node.name,
            ParentnodeName: node.parentNode !== null ? node.parentNode!.name : null
        }) as Option);
        updateOptions(options);
    }

    async function onEnterPressed(e: React.KeyboardEvent) {
        if(e.charCode === 13) {
            e.preventDefault();
            const response = await Fetcher.FetchNodeByName(input);
            const options = response.map((node: Node) => ({
                NodeId: node.id,
                NodeName: node.name,
                ParentnodeName: node.parentNode !== null ? node.parentNode!.name : null
            }) as Option);
            const sorted = options.sort((a: Option,b: Option) => (a.NodeName > b.NodeName) ? 1 : ((b.NodeName > a.NodeName) ? -1 : 0))
            console.log(options)
            updateOptions(options); // a way to avoide duplicate code? (onEnterPressed & onSearch)
        }
    }

    return (
        <div className="Filter">
            <form method="get">
                <input className="search field" type="text" placeholder="e.g. woman, coffee" 
                    onChange={e => onInputGiven(e.target.value)} onKeyPress={(e) => onEnterPressed(e)}/>
            </form>
            <button disabled={input === ""} className="submit button" type="submit" onClick={e => onSearch(e)}>Search</button>
            {(options.length > 0) ? <SearchResults options={options} onOptionSelected={onOptionSelected}/> : null }
            {selectedNode !== null ? <HierarchyBrowser startNode={selectedNode} activeFilters={props.activeFilters} onFiltersChanged={props.onFiltersChanged}/> : null }
        </div>
    )
}