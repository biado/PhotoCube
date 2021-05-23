import React, { useEffect, useState } from "react";
import { Filter } from "../Filter";
import { createFilter } from "../Middle/BottomDock/TagsetFilter";
import Fetcher from "../Middle/CubeBrowser/Fetcher";
import { Tag } from "./Tag";
import '../../css/LeftDock/TagFilter.css';

const TagFilterAdder = (props: {
    selectedTag: Tag,
    activeFilters: Filter[],
    onFiltersChanged: (filter: Filter) => void
}) => {
    const [selectedTag, updateSelection] = useState<Tag|null>(null);
    const [buttonDisabled, disableButton] = useState<boolean>(false);

    useEffect(() => {
        updateSelection(props.selectedTag);
    }, [props.selectedTag])

    const onButtonClick = () => {
        const filter: Filter = createFilter(selectedTag!.Name, selectedTag!.Id, "tag", "", "");
        if (!props.activeFilters.some(af => af.Id === filter.Id)) {
            props.onFiltersChanged(filter);
        }
    }

    return (
        <button className="add tag filter button" onClick={() => onButtonClick()}>Add filter</button>
    )
}

const SearchResults = (props: {
    options: Tag[], onTagOptionSelected: (e: React.ChangeEvent<HTMLSelectElement>) => void}) => {

    return(
        <div className="search results">
            <h5>{props.options.length} occurence(s) found:</h5>
            <select defaultValue="" onChange={e => props.onTagOptionSelected(e)} id="tag-dropdown">
                <option key={0} value="" disabled hidden>Select filter</option>
                {props.options.map(o => <option key={o.Id} value={JSON.stringify(o)}>{o.Name}</option>)}
            </select> 
        </div>
    )
}

export const TagSearcher = (props: {className: string, onFiltersChanged: (filter: Filter) => void, activeFilters: Filter[]}) => {
    const [input, updateInput] = useState<string>("");
    const [options, updateOptions] = useState<Tag[]>([]);
    const [selectedTag, updateSelection] = useState<Tag|null>(null);

    const onInputGiven = (input: string) => {
        updateInput(input);
    }

    const onTagOptionSelected = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const selected: Tag = JSON.parse(e.currentTarget.value);
        updateSelection(selected);
    }

    async function onSearch(e: React.MouseEvent<HTMLButtonElement, MouseEvent>){
        e.preventDefault();
        const response = await Fetcher.FetchTagByName(input);
        updateOptions(response);
    }

    return (
        <div className="TagSearcher">
            <form method="get">
                <input className="search field" type="text" placeholder="Search tags" 
                    onChange={e => onInputGiven(e.target.value)}/>
            </form>
            <button disabled={input === ""} className="submit button" type="submit" onClick={e => onSearch(e)}>Search</button>
            {(options.length > 0) ? <SearchResults options={options} onTagOptionSelected={onTagOptionSelected}/> : null }
            { (selectedTag !== null) ? <TagFilterAdder selectedTag={selectedTag} activeFilters={props.activeFilters} onFiltersChanged={props.onFiltersChanged}/> : null }
        </div>
    )
}