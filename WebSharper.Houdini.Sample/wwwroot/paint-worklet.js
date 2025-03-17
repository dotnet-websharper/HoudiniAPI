class StripedBackground {
    static get inputProperties() {
        return ["--stripe-color", "--stripe-width"];
    }

    paint(ctx, size, properties) {
        const stripeColor = properties.get("--stripe-color").toString() || "rgb(135,206,250)";
        const stripeWidth = parseInt(properties.get("--stripe-width")) || 10;

        ctx.fillStyle = stripeColor;
        for (let x = 0; x < size.width; x += stripeWidth * 2) {
            ctx.fillRect(x, 0, stripeWidth, size.height);
        }
    }
}

registerPaint("striped-background", StripedBackground);
