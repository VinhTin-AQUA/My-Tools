# QuicktoolsFe

- https://optimus.openng.org/listbox

## Prompt

```txt
Trong angular,
Sử dụng PrimeNG Theme (styled mode) làm nguồn màu duy nhất cho toàn bộ giao diện.

Toàn bộ màu phải lấy từ PrimeNG CSS variables (--p-*).

Quy tắc sử dụng màu:

 primary: {
    color: '{green.700}',
    inverseColor: '#ffffff',
    hoverColor: '{green.800}',
    activeColor: '{green.900}',
},

danger: {
    color: '{red.600}',
    inverseColor: '#ffffff',
    hoverColor: '{red.700}',
    activeColor: '{red.800}',
},

highlight: {
    background: '{green.100}',
    focusBackground: '{green.200}',
    color: '{green.950}',
    focusColor: '#000000',
},

// Surface - nền đen tuyền cho light mode
surface: {
    background: '#000000',
    card: '#0a0a0a',
    border: '#1a1a1a',
    hover: '#1a1a1a',
},

// Text màu sáng trên nền đen
text: {
    color: '#e5e5e5',
    hoverColor: '#ffffff',
    mutedColor: '#a3a3a3',
},



đồng thời phải sử dụng tailwind class inline html về bố cục, kích thước, .... riêng với màu sắc phải sử dụng custom như đã liệt kê ở trên và code trong css riêng. Lưu ý cho cả giao diện mobile

Một giao diện có thể đổi toàn bộ theme chỉ bằng cách thay đổi PrimeNG preset/semantic config mà không cần sửa HTML/CSS.

Đồng thời phải sử dụng các control hiện đại, ví dụ @for, @if

mô tả giao diện:

đã có sẵn ts và các hàm
    readonly searchTerm = signal('');

    showAddDialog = signal(false);
    showUpdateDialog = signal(false);

    selectedIcon = signal<IconModel | null>(null);
    page = signal<number>(1);
    pageSize = signal<number>(1);

    icons = signal<IconModel[]>([
        // {
        //     id: '',
        //     name: 'Funny Dance',
        //     iconType: 0,
        //     url: 'https://media4.giphy.com/media/v1.Y2lkPTE5NGEwMzQ5dXJ5YnU4b2FqencyZjJ4OXlvbXVzcGpjMjE0eWx2MGJ6Y2F0eW9yZSZlcD12MV9naWZzX2dpZklkJmN0PXM/9Ztp68jWLQE5DrJAZM/200.gif',
        // },
    ]);

    private messageService = inject(MessageService);

    constructor(private webuiService: WebuiService) {}

    async ngOnInit() {
        await this.searchicons();
    }

    /* ========================= api actions ========================= */

    async searchicons() {
        try {
            const request: SearchIconRequest = {
                keyword: this.searchTerm(),
                page: this.page(),
                pageSize: this.pageSize(),
            };

            const r = await this.webuiService.callJson<SearchIconResponse>('searchIcons', request);

            console.log(r);

            if (r) {
                this.icons.set(r.items);
                 console.log(this.icons());
            } else {
                this.messageService.add({
                    severity: 'error',
                    summary: 'Get icons failed',
                    detail: '',
                });
            }
        } catch (ex) {
            console.log(ex);

            this.messageService.add({
                severity: 'error',
                summary: 'Get icons failed',
                detail: '',
            });
        }
    }

    async deleteIcon(icon: IconModel) {
        const deleteIconRequest: DeleteIconRequest = {
            id: icon.id,
        };
        const r = await this.webuiService.callJson<boolean>('deleteIcon', deleteIconRequest);
        console.log(r);

        if (r) {
            this.messageService.add({
                severity: 'success',
                summary: 'Delete icon successfully',
                detail: icon.name,
            });
        } else {
            this.messageService.add({
                severity: 'error',
                summary: 'Delete icon failed',
                detail: icon.name,
            });
        }
    }

    async downloadIcon(icon: IconModel): Promise<void> {
        try {
            const response = await fetch(icon.url);

            if (!response.ok) {
                throw new Error(`Download failed: ${response.status}`);
            }

            const blob = await response.blob();
            const blobUrl = URL.createObjectURL(blob);

            const anchor = document.createElement('a');

            anchor.href = blobUrl;
            anchor.download = `${icon.name}.gif`;

            document.body.appendChild(anchor);
            anchor.click();
            anchor.remove();

            URL.revokeObjectURL(blobUrl);
        } catch (error) {
            console.error('Unable to download icon:', error);
        }
    }

    /* ========================= ui actions ========================= */
    openAddDialog(): void {
        this.showAddDialog.set(true);
    }

    closeAddDialog(): void {
        this.showAddDialog.set(false);
    }

    openUpdateDialog(icon: IconModel): void {
        this.selectedIcon.set(icon);
        this.showUpdateDialog.set(true);
    }

    closeUpdateDialog(): void {
        this.showUpdateDialog.set(false);
        this.selectedIcon.set(null);
    }

giờ code giao diện html theo yêu cầu ở trên để liệt kê danh sách icons theo dạng grid, có pagination, có search, nói chung mọi thứ sử dụng với các hàm ts

```