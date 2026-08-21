const form = document.querySelector("#proposalForm");
const resetButton = document.querySelector("#resetForm");
const sampleButton = document.querySelector("#loadRiskSample");
const verdict = document.querySelector("#verdict");
const verdictLabel = document.querySelector("#verdictLabel");
const verdictMessage = document.querySelector("#verdictMessage");
const resultList = document.querySelector("#resultList");
const riskMeterFill = document.querySelector("#riskMeterFill");
const riskScore = document.querySelector("#riskScore");
const latencyBadge = document.querySelector("#latencyBadge");
const summaryStudent = document.querySelector("#summaryStudent");
const summaryResource = document.querySelector("#summaryResource");
const summaryExposure = document.querySelector("#summaryExposure");
const validationState = document.querySelector("#validationState");
const alertLevel = document.querySelector("#alertLevel");
const nextStep = document.querySelector("#nextStep");
const qualityScore = document.querySelector("#qualityScore");
const qualityMeter = document.querySelector("#qualityMeter");
const qualityCopy = document.querySelector("#qualityCopy");
const budgetPosture = document.querySelector("#budgetPosture");
const resourcePosture = document.querySelector("#resourcePosture");
const ethicsPosture = document.querySelector("#ethicsPosture");
const requestPreview = document.querySelector("#requestPreview");

const defaults = {
    studentName: "Navida Perera",
    resourceRequested: "Survey Platform",
    quantity: 1,
    ethicsRiskCountry: "Sri Lanka",
    estimatedBudgetUSD: 1500,
    researchArea: "Digital Systems"
};

const highRiskSample = {
    studentName: "Navida Perera",
    resourceRequested: "Advanced GPU Cluster",
    quantity: 4,
    ethicsRiskCountry: "Bermuda",
    estimatedBudgetUSD: 9000,
    researchArea: "Applied AI"
};

form.addEventListener("submit", async (event) => {
    event.preventDefault();

    const startedAt = performance.now();
    const proposal = readForm();

    latencyBadge.textContent = "Analysing";
    renderVerdict("approved", "Analysing", "Compliance engine is evaluating this submission.", "...");
    renderRisk(18, "var(--blue)");
    renderMeta("Checking", "Pending", "Analyse");
    setLoadingState(true);

    try {
        const response = await fetch("/api/proposal/analyse", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(proposal)
        });

        const payload = await response.json();
        renderResult(payload, Math.round(performance.now() - startedAt));
    } catch {
        renderSystemError();
    } finally {
        setLoadingState(false);
    }
});

form.addEventListener("input", renderDossier);
form.addEventListener("change", renderDossier);

resetButton.addEventListener("click", () => {
    writeForm(defaults);
    renderIdle();
    renderDossier();
});

sampleButton.addEventListener("click", () => {
    writeForm(highRiskSample);
    form.requestSubmit();
});

function readForm() {
    const data = new FormData(form);

    return {
        studentName: data.get("studentName")?.toString().trim(),
        resourceRequested: data.get("resourceRequested")?.toString().trim(),
        quantity: Number(data.get("quantity")),
        ethicsRiskCountry: data.get("ethicsRiskCountry")?.toString().trim(),
        estimatedBudgetUSD: Number(data.get("estimatedBudgetUSD")),
        researchArea: data.get("researchArea")?.toString().trim()
    };
}

function writeForm(values) {
    Object.entries(values).forEach(([key, value]) => {
        const field = form.elements.namedItem(key);

        if (field) {
            field.value = value;
        }
    });
}

function renderResult(payload, latencyMs) {
    latencyBadge.textContent = `${latencyMs} ms`;

    if (payload.complianceStatus === "ValidationFailed") {
        renderVerdict("failed", "Validation Failed", "Submission needs correction before screening.", "!");
        renderRisk(38, "var(--red)");
        renderMeta("Needs fixes", "Validation", "Correct fields");
        renderItems(payload.validationErrors, "Validation", (issue) => issue.field, (issue) => issue.message);
        return;
    }

    if (payload.complianceStatus === "ReviewRequired") {
        const highSeverityCount = payload.complianceAlerts.filter((alert) => alert.severity === "High").length;
        renderVerdict("review", "Review Required", `${payload.complianceAlerts.length} compliance alert(s) detected.`, "!");
        renderRisk(highSeverityCount > 0 ? 88 : 54, highSeverityCount > 0 ? "var(--red)" : "var(--amber)");
        renderMeta("Passed", highSeverityCount > 0 ? "High" : "Low", "Manual review");
        renderItems(payload.complianceAlerts, "Alert", (alert) => `${alert.ruleCode} · ${alert.severity}`, (alert) => alert.message);
        return;
    }

    renderVerdict("approved", "Approved", "No compliance risks detected.", "✓");
    renderRisk(8, "var(--green)");
    renderMeta("Clean", "None", "Blind match");
    resultList.innerHTML = `
        <article>
            <span>OK</span>
            <p><strong>Clear for blind matching</strong>This proposal passed validation and did not trigger budget, resource, or ethics alerts.</p>
        </article>
    `;
}

function renderVerdict(state, label, message, icon) {
    verdict.className = `decision ${state}`;
    verdict.querySelector(".decision-mark").textContent = icon;
    verdictLabel.textContent = label;
    verdictMessage.textContent = message;
}

function renderRisk(width, color) {
    riskMeterFill.style.width = `${width}%`;
    riskMeterFill.style.background = color;
    riskScore.textContent = String(width).padStart(2, "0");
}

function renderItems(items, prefix, heading, message) {
    resultList.innerHTML = items.map((item, index) => `
        <article>
            <span>${String(index + 1).padStart(2, "0")}</span>
            <p><strong>${prefix}: ${escapeHtml(heading(item))}</strong>${escapeHtml(message(item))}</p>
        </article>
    `).join("");
}

function renderIdle() {
    latencyBadge.textContent = "Ready";
    renderVerdict("approved", "Approved", "No compliance risks detected.", "✓");
    renderRisk(8, "var(--green)");
    renderMeta("Clean", "None", "Blind match");
    resultList.innerHTML = `
        <article class="empty-state">
            <span>01</span>
            <p>Submit a proposal to view validation issues and rule alerts.</p>
        </article>
    `;
}

function renderSystemError() {
    latencyBadge.textContent = "Error";
    renderVerdict("failed", "System Error", "The compliance service could not be reached.", "!");
    renderRisk(100, "var(--red)");
    renderMeta("Unknown", "System", "Retry");
    resultList.innerHTML = `
        <article>
            <span>!</span>
            <p><strong>Request failed</strong>Check that the API project is running and try again.</p>
        </article>
    `;
}

function setLoadingState(isLoading) {
    form.querySelectorAll("button, input, select").forEach((field) => {
        field.disabled = isLoading;
    });
}

function renderDossier() {
    const proposal = readForm();
    const quantity = Number.isFinite(proposal.quantity) ? proposal.quantity : 0;
    const budget = Number.isFinite(proposal.estimatedBudgetUSD) ? proposal.estimatedBudgetUSD : 0;
    const highExposure = proposal.ethicsRiskCountry?.toLowerCase() === "bermuda"
        || proposal.resourceRequested?.toLowerCase() === "advanced gpu cluster" && quantity > 3
        || budget > 5000;

    summaryStudent.textContent = proposal.studentName || "Not set";
    summaryResource.textContent = proposal.resourceRequested || "Not set";
    summaryExposure.textContent = highExposure ? "Elevated" : "Low";

    renderQuality(proposal);
    renderRulePosture(proposal, quantity, budget);
    renderRequestPreview(proposal);
}

function renderMeta(validation, alert, step) {
    validationState.textContent = validation;
    alertLevel.textContent = alert;
    nextStep.textContent = step;
}

function renderQuality(proposal) {
    const requiredValues = [
        proposal.studentName,
        proposal.resourceRequested,
        proposal.quantity > 0 ? proposal.quantity : "",
        proposal.ethicsRiskCountry,
        proposal.estimatedBudgetUSD > 0 ? proposal.estimatedBudgetUSD : ""
    ];
    const completed = requiredValues.filter(Boolean).length;
    const score = Math.round((completed / requiredValues.length) * 100);
    const isReady = score === 100;

    qualityScore.textContent = `${score}%`;
    qualityMeter.style.width = `${score}%`;
    qualityMeter.style.background = isReady
        ? "linear-gradient(90deg, #0a8f63, #2859f6)"
        : "linear-gradient(90deg, #a76600, #d8a136)";
    qualityCopy.textContent = isReady
        ? "Ready for compliance analysis."
        : "Complete all required fields before screening.";
}

function renderRulePosture(proposal, quantity, budget) {
    const budgetFlagged = budget > 5000;
    const resourceFlagged = proposal.resourceRequested?.toLowerCase() === "advanced gpu cluster" && quantity > 3;
    const ethicsFlagged = proposal.ethicsRiskCountry?.toLowerCase() === "bermuda";

    setPosture(budgetPosture, budgetFlagged ? "Budget review" : "Budget clear", budgetFlagged ? "watch" : "");
    setPosture(resourcePosture, resourceFlagged ? "Resource alert" : "Resource clear", resourceFlagged ? "flagged" : "");
    setPosture(ethicsPosture, ethicsFlagged ? "Ethics alert" : "Ethics clear", ethicsFlagged ? "flagged" : "");
}

function setPosture(element, text, className) {
    element.textContent = text;
    element.className = className;
}

function renderRequestPreview(proposal) {
    requestPreview.textContent = JSON.stringify({
        studentName: proposal.studentName || "",
        resourceRequested: proposal.resourceRequested || "",
        quantity: Number.isFinite(proposal.quantity) ? proposal.quantity : 0,
        ethicsRiskCountry: proposal.ethicsRiskCountry || "",
        estimatedBudgetUSD: Number.isFinite(proposal.estimatedBudgetUSD) ? proposal.estimatedBudgetUSD : 0,
        researchArea: proposal.researchArea || ""
    }, null, 2);
}

function escapeHtml(value) {
    return String(value)
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}

renderDossier();
