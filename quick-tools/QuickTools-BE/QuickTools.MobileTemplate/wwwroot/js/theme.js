window.theme = {
    setVariables: function (cssVariables) {
        const root = document.documentElement;

        cssVariables
            .split(';')
            .map(x => x.trim())
            .filter(x => x.length > 0)
            .forEach(variable => {
                const separatorIndex = variable.indexOf(':');

                if (separatorIndex === -1) {
                    return;
                }

                const name = variable.substring(0, separatorIndex).trim();
                const value = variable.substring(separatorIndex + 1).trim();

                root.style.setProperty(name, value);
            });
    }
};
