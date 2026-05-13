import { Component } from '@angular/core';

// download
@Component({
    selector: 'lib-icon-download',
    standalone: true,
    template: `
        <svg
            viewBox="0 0 25 24"
            fill="currentColor"
            stroke="currentColor"
            xmlns="http://www.w3.org/2000/svg"
        >
            <path
                d="M13.1739 4C13.1739 3.58579 12.8381 3.25 12.4239 3.25C12.0096 3.25 11.6739 3.58579 11.6739 4L11.6739 10.6254L7.79691 10.6254C7.49352 10.6254 7.22001 10.8082 7.10395 11.0885C6.98789 11.3688 7.05215 11.6914 7.26674 11.9059L11.8763 16.5126C12.0132 16.6587 12.2079 16.75 12.4239 16.75C12.6566 16.75 12.8646 16.644 13.0021 16.4776L17.5771 11.9059C17.7917 11.6915 17.8559 11.3688 17.7399 11.0885C17.6238 10.8082 17.3503 10.6254 17.0469 10.6254H13.1739L13.1739 4Z"
            />
            <path
                d="M5.17188 16C5.17188 15.5858 4.83609 15.25 4.42188 15.25C4.00766 15.25 3.67188 15.5858 3.67188 16V18.5C3.67188 19.7426 4.67923 20.75 5.92188 20.75H18.9227C20.1654 20.75 21.1727 19.7426 21.1727 18.5V16C21.1727 15.5858 20.837 15.25 20.4227 15.25C20.0085 15.25 19.6727 15.5858 19.6727 16V18.5C19.6727 18.9142 19.337 19.25 18.9227 19.25H5.92188C5.50766 19.25 5.17188 18.9142 5.17188 18.5V16Z"
            />
        </svg>
    `,
})
export class DownloadIconComponent {}

// tick

@Component({
    selector: 'lib-icon-tick',
    standalone: true,
    template: `
        <svg
            viewBox="0 0 24 24"
            fill="currentColor"
            stroke="currentColor"
            xmlns="http://www.w3.org/2000/svg"
        >
            <path
                d="M19.5455 6.4965C19.9848 6.93584 19.9848 7.64815 19.5455 8.08749L10.1286 17.5043C9.6893 17.9437 8.97699 17.9437 8.53765 17.5043L4.45451 13.4212C4.01517 12.9819 4.01516 12.2695 4.4545 11.8302C4.89384 11.3909 5.60616 11.3909 6.0455 11.8302L9.33315 15.1179L17.9545 6.4965C18.3938 6.05716 19.1062 6.05716 19.5455 6.4965Z"
            />
        </svg>
    `,
})
export class TickIconComponent {}

// trash
@Component({
    selector: 'lib-icon-trash',
    standalone: true,
    template: `
        <svg
            viewBox="0 0 24 24"
            fill="currentColor"
            stroke="currentColor"
            xmlns="http://www.w3.org/2000/svg"
        >
            <path
                d="M7.99902 4.25C7.99902 3.00736 9.00638 2 10.249 2H13.749C14.9917 2 15.999 3.00736 15.999 4.25V5H18.498C19.7407 5 20.748 6.00736 20.748 7.25C20.748 8.28958 20.043 9.16449 19.085 9.42267C18.8979 9.4731 18.7011 9.5 18.498 9.5H5.5C5.29694 9.5 5.10016 9.4731 4.91303 9.42267C3.95503 9.16449 3.25 8.28958 3.25 7.25C3.25 6.00736 4.25736 5 5.5 5H7.99902V4.25ZM14.499 5V4.25C14.499 3.83579 14.1632 3.5 13.749 3.5H10.249C9.83481 3.5 9.49902 3.83579 9.49902 4.25V5H14.499Z"
            />
            <path
                d="M4.97514 10.4578L5.54076 19.8848C5.61205 21.0729 6.59642 22 7.78672 22H16.2113C17.4016 22 18.386 21.0729 18.4573 19.8848L19.0229 10.4578C18.8521 10.4856 18.6767 10.5 18.498 10.5H5.5C5.32131 10.5 5.146 10.4856 4.97514 10.4578ZM10.774 13.4339L10.9982 17.9905C11.0185 18.4042 10.6996 18.7561 10.2859 18.7764C9.8722 18.7968 9.52032 18.4779 9.49997 18.0642L9.27581 13.5076C9.25546 13.0938 9.57434 12.742 9.98805 12.7216C10.4018 12.7013 10.7536 13.0201 10.774 13.4339ZM14.0101 12.7216C14.4238 12.742 14.7427 13.0938 14.7223 13.5076L14.4982 18.0642C14.4778 18.4779 14.1259 18.7968 13.7122 18.7764C13.2985 18.7561 12.9796 18.4042 13 17.9905L13.2241 13.4339C13.2445 13.0201 13.5964 12.7013 14.0101 12.7216Z"
            />
        </svg>
    `,
})
export class TrashIconComponent {}

// loading
@Component({
    selector: 'lib-icon-trash',
    standalone: true,
    template: `
        <svg
            viewBox="0 0 24 24"
            xmlns="http://www.w3.org/2000/svg"
            data-iconid="346058"
            data-svgname="Loader 3 fill"
            stroke="currentColor"
        >
            <g>
                <path fill="none"></path>
                <path
                    fill="currentColor"
                    d="M3.055 13H5.07a7.002 7.002 0 0 0 13.858 0h2.016a9.001 9.001 0 0 1-17.89 0zm0-2a9.001 9.001 0 0 1 17.89 0H18.93a7.002 7.002 0 0 0-13.858 0H3.055z"
                ></path>
            </g>
        </svg>
    `,
})
export class LoadingIconComponent {}

// error
@Component({
    selector: 'lib-icon-trash',
    standalone: true,
    template: `
        <svg
            xmlns="http://www.w3.org/2000/svg"
            fill="currentColor"
            stroke="currentColor"
            viewBox="0 0 24 24"
            stroke-width="1.5"
        >
            <path
                stroke-linecap="round"
                stroke-linejoin="round"
                d="M12 9v3.75m9-.75a9 9 0 1 1-18 0 9 9 0 0 1 18 0Zm-9 3.75h.008v.008H12v-.008Z"
            />
        </svg>
    `,
})
export class ErrorIconComponent {}

// common
@Component({
    selector: 'lib-icon-trash',
    standalone: true,
    template: `<svg
        fill="currentColor"
        stroke="currentColor"
        viewBox="0 0 64 64"
        enable-background="new 0 0 64 64"
        version="1.1"
        xml:space="preserve"
        xmlns="http://www.w3.org/2000/svg"
        xmlns:xlink="http://www.w3.org/1999/xlink"
        data-iconid="381351"
        data-svgname="Electronic camera photo"
    >
        <g id="_x31_-smartphone"></g>

        <g id="_x32_-smartTV"></g>

        <g id="_x33_-telepon"></g>

        <g id="_x34_-computer"></g>

        <g id="_x35_-kalkulator"></g>

        <g id="_x36_-radio"></g>

        <g id="_x37_-hadset"></g>

        <g id="_x38_-usbflashdisk"></g>

        <g id="_x39_-camera">
            <g>
                <path
                    d="M43.938,18.6L42.056,9.36H21.944L20.062,18.6H18V15h-8v3.6H6v34.186C6,57.315,9.252,61,13.25,61h37.5    c3.998,0,7.25-3.685,7.25-8.214V18.6H43.938z M23.577,11.36h16.846l1.476,7.239H22.102L23.577,11.36z M12,17h4v1.6h-4V17z     M56,52.786C56,56.212,53.645,59,50.75,59h-37.5C10.355,59,8,56.212,8,52.786V20.6h48V52.786z"
                ></path>

                <path
                    d="M32,52c6.617,0,12-5.383,12-12s-5.383-12-12-12s-12,5.383-12,12S25.383,52,32,52z M32,30c5.514,0,10,4.486,10,10    s-4.486,10-10,10s-10-4.486-10-10S26.486,30,32,30z"
                ></path>

                <path
                    d="M32,47c3.859,0,7-3.14,7-7s-3.141-7-7-7s-7,3.14-7,7S28.141,47,32,47z M32,35c2.757,0,5,2.243,5,5s-2.243,5-5,5    s-5-2.243-5-5S29.243,35,32,35z"
                ></path>

                <path
                    d="M49,30c2.206,0,4-1.794,4-4s-1.794-4-4-4s-4,1.794-4,4S46.794,30,49,30z M49,24c1.103,0,2,0.897,2,2s-0.897,2-2,2    s-2-0.897-2-2S47.897,24,49,24z"
                ></path>
            </g>
        </g>

        <g id="_x31_0-printer"></g>

        <g id="_x31_1-powerbank"></g>

        <g id="_x31_2-kulkas"></g>

        <g id="_x31_3-modem"></g>

        <g id="_x31_4-tablet"></g>

        <g id="_x31_5-gamebot"></g>

        <g id="_x31_6-wasing_machine"></g>

        <g id="_x31_7-handdryer"></g>

        <g id="_x31_8-blender"></g>

        <g id="_x31_9-controller"></g>

        <g id="_x32_0-michrophone"></g>

        <g id="_x32_1-lamp"></g>
    </svg>`,
})
export class CameraIconComponent {}

// common
@Component({
    selector: 'lib-icon-trash',
    standalone: true,
    template: ``,
})
export class CommonIconComponent {}
