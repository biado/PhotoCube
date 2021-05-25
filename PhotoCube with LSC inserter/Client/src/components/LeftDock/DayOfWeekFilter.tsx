import React from 'react';
import { Filter } from '../Filter';
import Fetcher from '../Middle/CubeBrowser/Fetcher';
import { Tag } from './Tag';
import { createFilter } from '../Middle/BottomDock/TagsetFilter';
import '../../css/LeftDock/DayOfWeekFilter.css';

/**
 * Component for applying tag filters from Day of week tagset.
 * The tag filters are applied when checking/unchecking the checkboxes.
 * IMPORTANT: Tag filters applied from this section will result in OR search.
 */
export default class DayOfWeekFilter extends React.Component<{
    onFiltersChanged: (filters: Filter) => void,
    activeFilters: Filter[],
    onFilterRemoved: (filterId: number) => void
}>{
    state = {
        daysOfWeek: [],
        dayNames: ["M", "T", "W", "T", "F", "S", "S"]
    }

    render() {
        return (
            <div className="dow filter">
                <ul>
                    {this.state.daysOfWeek.map((dow: Tag) => 
                        <li key={dow.Id}>{this.renderDow(dow)}</li>
                    )}
                </ul>
            </div>
        );
    }

    componentDidMount() {
        this.renderDaysOfWeek();
    }

    /**
     * Fetches tags in Day of Week tagset from the server, and presents them with a checkbox.
     */
    private async renderDaysOfWeek() {
        const DOW: Tag[] = await Fetcher.FetchTagsByTagsetName("Day of week (number)")
        this.setState({daysOfWeek: DOW})
    }

    /**
     * Renders tags and checkboxes.
     */
    private renderDow(dowTag: Tag) {
        let inputElement = <input
            type="checkbox"
            name={dowTag.Name}
            value={dowTag.Id}
            onChange={e => this.onChange(e)} />;
        let result = <div className="dow checkbox">
                {inputElement}
                <p>{this.state.dayNames[parseInt(dowTag.Name)-1]}</p>
            </div>
        return result;
    }

    /**
     * If a checkbox is checked or unchecked, this method is called.
     * When checked: Adds a filter corresponding to the tag, and calls this.props.onFiltersChanged.
     * When unchecked: Removes the filter corresponding to the tag, and calls this.props.onFilterUnchecked.
     */
    private onChange(e: React.ChangeEvent<HTMLInputElement>) {
        if (e.target.checked) {
            let filter: Filter = createFilter(e.target.name, parseInt(e.target.value), "day of week", "", "");
            //Add filter
            if (!this.props.activeFilters.some(af => af.Id === filter.Id)) {
                this.props.onFiltersChanged(filter);
            }
        } else {
            let filter: Filter = createFilter(e.target.name, parseInt(e.target.value), "day of week", "", "");
            if (this.props.activeFilters.some(af => af.Id === filter.Id)) {
                this.props.onFilterRemoved(filter.Id);
            }
        }
    }
}