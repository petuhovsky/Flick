import React, {useEffect, useRef, useState} from 'react';
import PropTypes from 'prop-types';
import cl from "./SearchForm.module.css";
import SearchInput from "./SearchInput/SearchInput";
import SearchButton from "./SearchButton/SearchButton";
import ClearButton from "./ClearButton/ClearButton";

const SearchForm = ({initialQuery, onSubmit}) => {
    const input = useRef();
    const [value, setValue] = useState(initialQuery)
    const [showClear, setShowClear] = useState(false);

    useEffect(()=>{
        setValue(initialQuery);
    }, [initialQuery]);

    useEffect(() => {
        setShowClear(value);
    }, [value]);

    const clear = () => {
        setValue('');
        input.current.focus();
    };

    const change = event => {
        setValue(event.target.value);
    }

    const submit = event => {
        event.preventDefault();

        onSubmit(value);
    };

    return (
        <form className={cl.search_form} onSubmit={submit}>
            <div className={cl.search_input}>
                <SearchInput ref={input} autoFocus placeholder='Enter some keywords' type='text'
                             value={value} onChange={change} onEscape={clear}/>
                <ClearButton onClick={clear} style={showClear?{}:{display:'none'}} type='button'/>
            </div>
            <SearchButton type='submit'>GO</SearchButton>
        </form>
    );
};

SearchForm.propTypes = {
    initialQuery: PropTypes.string,
    setQuery: PropTypes.func,
    onSubmit: PropTypes.func,
};

export default SearchForm;