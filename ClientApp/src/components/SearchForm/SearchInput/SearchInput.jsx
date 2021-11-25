import React from 'react';
import PropTypes from 'prop-types';
import classes from './SearchInput.module.css'

const SearchInput = React.forwardRef(({ onEscape, ...props }, ref) => {
    const onKeyUp = event => {
        const escape_code = 27;
        if (event.keyCode === escape_code) {
            onEscape();
        }
    }

    return (
        <input ref={ref} {...props} className={classes.searchInput} onKeyUp={onKeyUp}/>
    );
});

SearchInput.propTypes = {
    value: PropTypes.string,
};

export default SearchInput;
