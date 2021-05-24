import React from 'react';
import { Filter } from '../Filter';
import Fetcher from '../Middle/CubeBrowser/Fetcher';
import { Tag } from './Tag';
import { createFilter } from '../Middle/BottomDock/TagsetFilter';

/**
 * Component for applying tag filters from Day of week tagset.
 * The tag filters are applied when checking/unchecking the checkboxes.
 * It also adds/removes from Active Filter list on the RightDock when interacting with the checkboxes.
 * IMPORTANT: Tag filters applied from this section will result in OR search.
 */
export default class DayOfWeekFilter extends React.Component<{
    onFiltersChanged: (filters: Filter) => void,
    activeFilters: Filter[],
    onFilterUnchecked: (filterId: number) => void
}>{
    state = {
        dayOfWeek: []
    }

    render() {
        return (
            <div className="scrollable2">
                {this.state.dayOfWeek}
            </div>
        );
    }

    componentDidMount() {
        this.renderDayOfWeeks();
    }

    /**
     * Fetches tags in Day of Week tagset from the server, and presents them with a checkbox.
     */
    private async renderDayOfWeeks() {
        let renderedDayOfWeek = await Fetcher.FetchTagsByTagsetName("Day of week (number)")
            .then((DOWs: Tag[]) => {
                return DOWs
                    .map((dow: Tag) => {                             //Map each day-of-week tag to JSX element
                        return <div key={dow.Id}>
                            <p className="dayOfWeekTagName">{this.renderDowTag(dow)}</p>
                        </div>;
                    });
            });
        this.setState({ dayOfWeek: renderedDayOfWeek });
    }

    /**
     * Renders tags and checkboxes.
     */
    private renderDowTag(dowTag: Tag) {
        let inputElement = <input
            type="checkbox"
            name={dowTag.Name}
            value={dowTag.Id}
            onChange={e => this.onChange(e)} />;
        let result = <div>
            <p>
                {inputElement}
                {this.mapNumberToDayOfWeek(dowTag.Name)}
            </p>
        </div>
        return result;
    }

    /**
     * Maps the Day of week (number) tags to Day of week (string) tags.
     */
    private mapNumberToDayOfWeek(dowInNumber: string) {
        switch (dowInNumber) {
            case "1":
                return "Monday";
            case "2":
                return "Tuesday";
            case "3":
                return "Wednesday";
            case "4":
                return "Thursday";
            case "5":
                return "Friday";
            case "6":
                return "Saturday";
            case "7":
                return "Sunday";
            default:
                break;
        }
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
                this.props.onFilterUnchecked(filter.Id);
            }
        }
    }
}