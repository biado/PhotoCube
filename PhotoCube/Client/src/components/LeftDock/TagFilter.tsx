import React, { useEffect, useState } from "react";
import { Filter } from "../Filter";
import { createFilter } from "../Middle/BottomDock/TagsetFilter";
import Fetcher from "../Middle/CubeBrowser/Fetcher";
import { Tag } from "./Tag";
import '../../css/LeftDock/TagFilter.css';

/**
 * Component for adding a selected Tag as a filter.
 */
const TagFilter = (props: {
    selectedTag: Tag,
    activeFilters: Filter[],
    onFiltersChanged: (filter: Filter) => void }) => {
    const [selectedTag, updateSelection] = useState<Tag|null>(null);
    const [buttonDisabled, disableButton] = useState<boolean>(false);

    useEffect(() => {
        updateSelection(props.selectedTag);
        if (!props.activeFilters.some(af => af.Id === props.selectedTag.id)) {
            disableButton(false);
        } else {
            disableButton(true);
        }
    }, [props.selectedTag])

    const onButtonClick = () => {
        const filter: Filter = createFilter(selectedTag!.name, selectedTag!.id, "tag");
        if (!props.activeFilters.some(af => af.Id === filter.Id)) {
            props.onFiltersChanged(filter);
            disableButton(true);
        }
    }

    return (
        <button disabled={buttonDisabled} onClick={() => onButtonClick()}>Add filter</button>
    )
}

/**
 * Component for displaying search results.
 */
const SearchResults = (props: {
    options: Tag[], onOptionSelected: (e: React.ChangeEvent<HTMLSelectElement>) => void}) => {
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
            <select value={selected} onChange={e => onOptionSelected(e)} className="tag result dropdown">
                <option key={0} value="">Select filter</option>
                {props.options.map(o => <option key={o.id} value={JSON.stringify(o)}>{o.name}</option>)}
            </select> 
        </div>
    )
}

/**
 * Component for searching tags and adding tag filters.
 * Consists of a search field and a search results dropdown component.
 */
export const TagSearcher = (props: { onFiltersChanged: (filter: Filter) => void, activeFilters: Filter[]}) => {
    const [input, updateInput] = useState<string>("");
    const [options, updateOptions] = useState<Tag[]>([]);
    const [selectedTag, updateSelection] = useState<Tag|null>(null);


    // useEffect(() => {
        // to implement dynamic searching
        // Is it possible to just retreive and store all tagnames?
        // then just filter that as a person types
    // })

    const onInputGiven = (input: string) => {
        updateInput(input);
    }

    const onOptionSelected = (e: React.ChangeEvent<HTMLSelectElement>) => {
        if(e.currentTarget.value != "") {
            const selected: Tag = JSON.parse(e.currentTarget.value);
            updateSelection(selected);
        }
    }

    async function onEnterPressed(e: React.KeyboardEvent) {
        if(e.charCode === 13) {
            e.preventDefault();
            const response = await Fetcher.FetchTagByName(input);
            const sorted = response.sort((a: Tag,b: Tag) => (a.name > b.name) ? 1 : ((b.name > a.name) ? -1 : 0))
            updateOptions(sorted);
            if (selectedTag !== null) { updateSelection(null); }
        } // a way to avoid duplicate code? (onEnterPressed & onSearch)
    }

    async function onSearch(e: React.MouseEvent<HTMLButtonElement, MouseEvent>){
        e.preventDefault();
        const response = await Fetcher.FetchTagByName(input);
        const sorted = response.sort((a: Tag,b: Tag) => (a.name > b.name) ? 1 : ((b.name > a.name) ? -1 : 0))
        updateOptions(sorted);
        if (selectedTag !== null) { updateSelection(null); }
    }

    return (
        <div className="TagSearcher">
            {/* <p>Search tags:</p>
            <form method="get">
                <input className="tag search field" type="text" placeholder="e.g. computer" 
                    onChange={e => onInputGiven(e.target.value)} onKeyPress={(e) => onEnterPressed(e)}/>
            </form> */}

            <form method="get">
                <input className="tag search field" type="text" placeholder="e.g. computer" 
                    onChange={e => onInputGiven(e.target.value)} onKeyPress={(e) => onEnterPressed(e)}/>
            </form>
            <button disabled={input === ""} className="submit button" type="submit" onClick={e => onSearch(e)}>Search</button>
            {(options.length > 0) ? <SearchResults options={options} onOptionSelected={onOptionSelected}/> : null }
            {(selectedTag !== null) ? <TagFilter selectedTag={selectedTag} activeFilters={props.activeFilters} onFiltersChanged={props.onFiltersChanged}/> : null }
        </div>
    )
}