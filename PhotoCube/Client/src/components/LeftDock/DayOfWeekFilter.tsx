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
        dayNames: ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"]
    }

    render() {
        return (
            <div className="dow filter">
                <ul>
                    {this.state.daysOfWeek.map((dow: Tag) => 
                        <li key={dow.id}>{this.renderDow(dow)}</li>
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
        //console.log("fetching from dayOfTheWeekFilter")
        const response = await Fetcher.FetchTagsByTagsetName("Day of week (number)")
        //console.log("response to dayoftheweekfilet", response)
        const DOW: Tag[] = response;
        //DOW.forEach(d => console.log("dow", d))
        DOW.sort((a,b) => parseInt(a.name) - parseInt(b.name));
        //DOW.forEach(d => console.log("dow", d))
        this.setState({daysOfWeek: DOW})
       /*  this.state.daysOfWeek.forEach(day => {
            console.log("this is a day", day)
        }) */
    }

    /**
     * Renders tags and checkboxes.
     */
    private renderDow(dowTag: Tag) {
        let inputElement = <input
            type="checkbox"
            name={dowTag.name}
            value={dowTag.id}
            onChange={e => this.onChange(e)} />;
        let result = <div className="dow checkbox">
                {inputElement}
                <p>{this.state.dayNames[parseInt(dowTag.name)-1].substring(0,1)}</p>
            </div>
        return result;
    }

    /**
     * If a checkbox is checked or unchecked, this method is called.
     * When checked: Adds a filter corresponding to the tag, and calls this.props.onFiltersChanged.
     * When unchecked: Removes the filter corresponding to the tag, and calls this.props.onFilterRemoved.
     */
    private onChange(e: React.ChangeEvent<HTMLInputElement>) {
        if (e.target.checked) {
            const filter: Filter = createFilter(e.target.name, parseInt(e.target.value), "day of week");
            //Add filter
            if (!this.props.activeFilters.some(af => af.name === e.target.name)) {
                this.props.onFiltersChanged(filter);
            }
        } else {
            //Remove filter
            const filterId = parseInt(e.target.value);
            if (this.props.activeFilters.some(af => af.Id === filterId)) {
                this.props.onFilterRemoved(filterId);
            }
        }
    }
}
